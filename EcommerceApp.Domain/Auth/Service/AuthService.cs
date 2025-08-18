using EcommerceApp.Domain.Auth.DTOs.Request;
using EcommerceApp.Domain.Auth.DTOs.Response;
using EcommerceApp.Domain.Auth.Interfaces;
using EcommerceApp.Domain.Auth.Mappings;
using EcommerceApp.Domain.Auth.Validations;
using EcommerceApp.Domain.User.Interfaces;
using EcommerceApp.Domain.User.Mappings;
using EcommerceApp.Model.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace EcommerceApp.Domain.Auth.Service
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IValidator<SignupRequest> _signUpUserValidator;
        private readonly IValidator<LoginRequest> _loginUserValidator;

        public AuthService(IAuthRepository authRepository, IConfiguration configuration, IValidator<SignupRequest> signUpUserValidator, IValidator<LoginRequest> loginUserValidator, IUserRepository userRepository)
        {
            _signUpUserValidator = signUpUserValidator;
            _authRepository = authRepository;
            _configuration = configuration;
            _loginUserValidator = loginUserValidator;
            _userRepository = userRepository;
        }

        public async Task<SignupResponse?> SignUpAsync(SignupRequest request)
        {
            // Validate the request using FluentValidation
            var validationResult = await _signUpUserValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                throw new ArgumentException(string.Join(", ", errors));
            }
            // Check if user already exists
            var userExists = await _authRepository.UserExistsAsync(request.Email, request.Username);
            if (userExists)
            {
                return null;
            }

            // Create user entity from DTO
            var user = request.ToEntity();

            // Hash password
            var passwordHasher = new PasswordHasher<UserModel>();
            user.Password = passwordHasher.HashPassword(user, request.Password);

            // Create user in database
            var userId = await _authRepository.CreateUserAsync(user);
            user.Id = userId;

            // Assign default role
            await _authRepository.AssignDefaultRoleAsync(userId);

            // Return response
            return user.ToSignupResponse(new[] { "User" });
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            // Validate the request using FluentValidation
            var validationResult = await _loginUserValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                throw new ArgumentException(string.Join(", ", errors));
            }
            Console.WriteLine($"Login attempt for: {request.Email}");
            
            var user = await _authRepository.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                Console.WriteLine("User not found");
                return null;
            }
            
            var passwordHasher = new PasswordHasher<UserModel>();
            var result = passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                Console.WriteLine("Password verification failed");
                return null;
            }

            // Create tokens with embedded user data
            var refreshToken = GenerateRefreshTokenForUser(user);
            var response = new LoginResponse
            {
                Token = CreateToken(user, refreshToken),
                RefreshToken = refreshToken,
            };

            return response;
        }

        public async Task<LoginResponse?> RefreshTokenAsync(int id, string refreshToken)
        {
            // Validate and extract user data from refresh token - NO DATABASE CALLS!
            var userData = ValidateAndExtractUserData(refreshToken);
            if (userData == null || userData.UserId != id)
            {
                return null;
            }

            // Generate new tokens using extracted user data - NO DATABASE CALLS!
            var newRefreshToken = GenerateRefreshTokenFromUserData(userData);
            var newToken = CreateTokenFromUserData(userData, newRefreshToken);

            return new LoginResponse
            {
                Token = newToken,
                RefreshToken = newRefreshToken
            };
        }

        private UserTokenData? ValidateAndExtractUserData(string refreshToken)
        {
            try
            {
                // Decrypt the refresh token to get the user data
                var decryptedPayload = DecryptRefreshToken(refreshToken);
                var userData = JsonSerializer.Deserialize<UserTokenData>(decryptedPayload);
                
                // Validate expiry time
                if (userData == null || userData.ExpiryTime < DateTime.UtcNow)
                {
                    return null;
                }

                return userData;
            }
            catch
            {
                return null;
            }
        }

        private string GenerateRefreshTokenForUser(UserModel user)
        {
            var userData = new UserTokenData
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Username = user.Username,
                IsActive = user.IsActive,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList(),
                ExpiryTime = DateTime.UtcNow.AddDays(7),
                TokenId = Guid.NewGuid().ToString()
            };

            var payload = JsonSerializer.Serialize(userData);
            return EncryptRefreshToken(payload);
        }

        private string GenerateRefreshTokenFromUserData(UserTokenData userData)
        {
            // Update expiry time and token ID for security
            userData.ExpiryTime = DateTime.UtcNow.AddDays(7);
            userData.TokenId = Guid.NewGuid().ToString();

            var payload = JsonSerializer.Serialize(userData);
            return EncryptRefreshToken(payload);
        }

        private string EncryptRefreshToken(string plainText)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Token"]!.Substring(0, 32));

            using var aes = Aes.Create();
            aes.Key = key;
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            using var ms = new MemoryStream();
            ms.Write(aes.IV, 0, aes.IV.Length);

            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        private string DecryptRefreshToken(string cipherText)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Token"]!.Substring(0, 32));
            var buffer = Convert.FromBase64String(cipherText);

            using var aes = Aes.Create();
            aes.Key = key;

            var iv = new byte[16];
            Array.Copy(buffer, 0, iv, 0, 16);
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            using var ms = new MemoryStream(buffer, 16, buffer.Length - 16);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);

            return sr.ReadToEnd();
        }

        private string CreateToken(UserModel user, string refreshToken)
        {
            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
            return CreateTokenWithClaims(user.Id, user.Name, user.Email, user.Username, roles, refreshToken);
        }

        private string CreateTokenFromUserData(UserTokenData userData, string refreshToken)
        {
            return CreateTokenWithClaims(userData.UserId, userData.Name, userData.Email, userData.Username, userData.Roles, refreshToken);
        }

        private string CreateTokenWithClaims(int userId, string name, string email, string username, List<string> roles, string refreshToken)
        {
            // Determine role flags
            var isAdmin = roles.Contains("Administrator") ? "1" : "0";
            var isSeller = roles.Contains("Seller") ? "1" : "0";
            var isUser = roles.Contains("User") ? "1" : "0";

            var claims = new List<Claim>
            {
                new Claim("name", name),
                new Claim("email", email),
                new Claim("username", username),
                new Claim("userId", userId.ToString()),
                new Claim("isAdmin", isAdmin),
                new Claim("isSeller", isSeller),
                new Claim("isUser", isUser),
                new Claim("refreshToken", refreshToken) // Embedded refresh token
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenKey = _configuration["JwtSettings:Token"];
            if (string.IsNullOrEmpty(tokenKey))
            {
                throw new InvalidOperationException("JWT Token key is not configured");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("JwtSettings:ExpiryTimeInMinutes")),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }

    // Data model for storing user information in encrypted refresh token
    public class UserTokenData
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; } = new();
        public DateTime ExpiryTime { get; set; }
        public string TokenId { get; set; } = string.Empty;
    }
}