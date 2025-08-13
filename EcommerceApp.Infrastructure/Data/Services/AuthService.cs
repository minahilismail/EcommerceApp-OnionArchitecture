using EcommerceApp.Application.DTOs.Request;
using EcommerceApp.Application.DTOs.Response;
using EcommerceApp.Application.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
namespace EcommerceApp.Infrastructure.Data.Services
{
    public class AuthService : IAuthService
    {
        
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
           
            _configuration = configuration;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return null;
            }


            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.Password, request.Password) == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return new LoginResponseDto
            {
                Token = CreateToken(user)
            };
        }

        public async Task<UserResponseDto?> RegisterAsync(RegisterRequestDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return null;

            // Start a transaction to ensure both User and UserRole are created together
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = new User();
                var hashedPassword = new PasswordHasher<User>()
                    .HashPassword(user, request.Password);
                user.Username = request.Username;
                user.Name = request.Name;
                user.Email = request.Email;
                user.Password = hashedPassword;

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync(); // Save to get the UserId

                // Assigning default role (User role with ID = 1)
                var defaultRoleId = 1; // "User" role
                var userRole = new UserRole
                {
                    UserId = user.Id,
                    RoleId = defaultRoleId,
                };

                await _context.Set<UserRole>().AddAsync(userRole);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // Get the role name for the response
                var role = await _context.Roles.FindAsync(defaultRoleId);

                return new UserResponseDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Username = user.Username,
                    Email = user.Email,
                    Roles = new List<string> { role?.Name ?? "User" },
                    CreatedOn = user.CreatedOn ?? DateTime.UtcNow
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return null;
            }
        }

        private string CreateToken(User user)
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

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JwtSettings:Token")!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescription = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("JwtSettings:Issuer"),
                audience: _configuration.GetValue<string>("JwtSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescription);
        }
    }
}
