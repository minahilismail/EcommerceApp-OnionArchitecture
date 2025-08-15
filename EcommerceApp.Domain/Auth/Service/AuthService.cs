using EcommerceApp.Domain.Auth.DTOs.Request;
using EcommerceApp.Domain.Auth.DTOs.Response;
using EcommerceApp.Domain.Auth.Interfaces;
using EcommerceApp.Domain.Auth.Mappings;
using EcommerceApp.Model.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EcommerceApp.Domain.Auth.Service
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IAuthRepository authRepository, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _configuration = configuration;
        }

        public async Task<SignupResponse?> SignUpAsync(SignupRequest request)
        {
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
            
            var token = CreateToken(user);
            Console.WriteLine($"Token created: {!string.IsNullOrEmpty(token)}");
            
            return user.ToLoginResponse(token);
        }

        private string CreateToken(UserModel user)
        {
            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

            // Determine role flags
            var isAdmin = roles.Contains("Administrator") ? "1" : "0";
            var isSeller = roles.Contains("Seller") ? "1" : "0";
            var isUser = roles.Contains("User") ? "1" : "0";

            var claims = new List<Claim>
            {
                new Claim("name", user.Name),
                new Claim("email", user.Email),
                new Claim("username", user.Username),
                new Claim("userId", user.Id.ToString()),
                new Claim("isAdmin", isAdmin),
                new Claim("isSeller", isSeller),
                new Claim("isUser", isUser)
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
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}