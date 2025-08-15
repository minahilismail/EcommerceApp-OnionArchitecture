using EcommerceApp.Domain.Auth.DTOs.Request;
using EcommerceApp.Domain.Auth.DTOs.Response;
using EcommerceApp.Domain.Auth.Interfaces;
using EcommerceApp.Model.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace EcommerceApp.Domain.Auth.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly string _connectionString;

        public AuthRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultSQLConnection")!;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            var user = await GetUserByEmailAsync(loginRequest.Email);
            
            if (user == null)
            {
                return null;
            }

            // Verify password
            var passwordHasher = new PasswordHasher<UserModel>();
            var result = passwordHasher.VerifyHashedPassword(user, user.Password, loginRequest.Password);
            
            if (result == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return new LoginResponse(); // Token will be set by service
        }

        public async Task<SignupResponse> SignUpAsync(SignupRequest signupRequest)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                // Check if user already exists
                const string checkUserSql = "SELECT COUNT(*) FROM [Users] WHERE Email = @Email OR Username = @Username";
                var userExists = await connection.QuerySingleAsync<int>(checkUserSql, 
                    new { Email = signupRequest.Email, Username = signupRequest.Username }, transaction);

                if (userExists > 0)
                {
                    return null;
                }

                // Hash password
                var passwordHasher = new PasswordHasher<UserModel>();
                var hashedPassword = passwordHasher.HashPassword(new UserModel(), signupRequest.Password);

                // Insert user
                const string insertUserSql = @"
                    INSERT INTO [Users] (Name, Username, Email, Password, IsActive, CreatedOn, UpdatedOn)
                    OUTPUT INSERTED.Id
                    VALUES (@Name, @Username, @Email, @Password, 1, @CreatedOn, @UpdatedOn)";

                var userId = await connection.QuerySingleAsync<int>(insertUserSql, new
                {
                    Name = signupRequest.Name,
                    Username = signupRequest.Username,
                    Email = signupRequest.Email,
                    Password = hashedPassword,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow
                }, transaction);

                // Assign default role (User role with ID = 1)
                const string insertUserRoleSql = @"
                    INSERT INTO UserRole (UserId, RoleId)
                    VALUES (@UserId, @RoleId)";

                await connection.ExecuteAsync(insertUserRoleSql, new
                {
                    UserId = userId,
                    RoleId = 1
                }, transaction);

                // Get role name
                const string getRoleSql = "SELECT Name FROM Roles WHERE Id = 1";
                var roleName = await connection.QuerySingleOrDefaultAsync<string>(getRoleSql, transaction: transaction);

                transaction.Commit();

                return new SignupResponse
                {
                    Id = userId,
                    Name = signupRequest.Name,
                    Username = signupRequest.Username,
                    Email = signupRequest.Email,
                    IsActive = true,
                    Roles = new[] { roleName ?? "User" }
                };
            }
            catch
            {
                transaction.Rollback();
                return null;
            }
        }

        public async Task<UserModel?> GetUserByEmailAsync(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            
            const string sql = @"
                SELECT u.Id, u.Name, u.Username, u.Email, u.Password, u.IsActive,
                       ur.UserId, ur.RoleId,
                       r.Id, r.Name
                FROM [Users] u
                LEFT JOIN UserRole ur ON u.Id = ur.UserId
                LEFT JOIN Roles r ON ur.RoleId = r.Id
                WHERE u.Email = @Email";

            var userDictionary = new Dictionary<int, UserModel>();

            await connection.QueryAsync<UserModel, UserRole, RoleModel, UserModel>(
                sql,
                (user, userRole, role) =>
                {
                    if (!userDictionary.TryGetValue(user.Id, out var existingUser))
                    {
                        existingUser = user;
                        existingUser.UserRoles = new List<UserRole>();
                        userDictionary.Add(user.Id, existingUser);
                    }

                    if (userRole != null && role != null)
                    {
                        userRole.Role = role;
                        existingUser.UserRoles.Add(userRole);
                    }

                    return existingUser;
                },
                new { Email = email },
                splitOn: "UserId,Id");

            return userDictionary.Values.FirstOrDefault();
        }

        public async Task<int> CreateUserAsync(UserModel user)
        {
            using var connection = new SqlConnection(_connectionString);
            
            const string sql = @"
                INSERT INTO [Users] (Name, Username, Email, Password, IsActive, CreatedOn, UpdatedOn)
                OUTPUT INSERTED.Id
                VALUES (@Name, @Username, @Email, @Password, @IsActive, @CreatedOn, @UpdatedOn)";

            return await connection.QuerySingleAsync<int>(sql, user);
        }

        public async Task<bool> UserExistsAsync(string email, string username)
        {
            using var connection = new SqlConnection(_connectionString);
            
            const string sql = @"
                SELECT COUNT(*) 
                FROM [Users] 
                WHERE Email = @Email OR Username = @Username";

            var count = await connection.QuerySingleAsync<int>(sql, new { Email = email, Username = username });
            return count > 0;
        }

        public async Task<bool> AssignDefaultRoleAsync(int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            
            const string sql = @"
                INSERT INTO UserRole (UserId, RoleId)
                VALUES (@UserId, @RoleId)";

            var rowsAffected = await connection.ExecuteAsync(sql, new { UserId = userId, RoleId = 1 }); // Default "User" role
            return rowsAffected > 0;
        }
    }
}
