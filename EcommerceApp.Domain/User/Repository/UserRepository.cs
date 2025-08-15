using Dapper;
using EcommerceApp.Domain.User.DTOs.Request;
using EcommerceApp.Domain.User.DTOs.Response;
using EcommerceApp.Domain.User.Interfaces;
using EcommerceApp.Model.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Domain.User.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultSQLConnection")!;
        }
        public async Task<IEnumerable<RoleModel>> GetRoles()
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT *
                FROM Roles r 
                ORDER BY r.Id";

            return await connection.QueryAsync<RoleModel>(sql);
        }

        public async Task<bool> UpdateUserRole(UpdateUserRoles updateUserRoles)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();
            
            try
            {
                // First, remove all existing roles for the user
                const string deleteSql = @"
                    DELETE FROM UserRole 
                    WHERE UserId = @UserId";
                
                await connection.ExecuteAsync(deleteSql, new { UserId = updateUserRoles.Id }, transaction);

                // Then, add the new roles
                if (updateUserRoles.RoleIds != null && updateUserRoles.RoleIds.Length > 0)
                {
                    const string insertSql = @"
                        INSERT INTO UserRole (UserId, RoleId) 
                        VALUES (@UserId, @RoleId)";

                    var userRoleData = updateUserRoles.RoleIds.Select(roleId => new 
                    { 
                        UserId = updateUserRoles.Id, 
                        RoleId = roleId 
                    });

                    await connection.ExecuteAsync(insertSql, userRoleData, transaction);
                }

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }

        public async Task<IEnumerable<UserModel>> GetUsersAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
                SELECT u.Id, u.Name, u.Username, u.Email, u.IsActive,
                       ur.UserId, ur.RoleId,
                       r.Id, r.Name
                FROM [Users] u
                LEFT JOIN UserRole ur ON u.Id = ur.UserId
                LEFT JOIN Roles r ON ur.RoleId = r.Id
                ORDER BY u.Id";

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
                splitOn: "UserId,Id");

            return userDictionary.Values;
        }

        public async Task<bool> UpdateUser(int id, UpdateUser user)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                // Check if user exists
                const string checkUserSql = "SELECT COUNT(*) FROM [Users] WHERE Id = @Id";
                var userExists = await connection.QuerySingleAsync<int>(checkUserSql, new { Id = id }, transaction);

                if (userExists == 0)
                {
                    return false;
                }

                // Check for duplicate email or username (excluding current user)
                const string duplicateCheckSql = @"
                    SELECT COUNT(*) FROM [Users] 
                    WHERE Id != @Id AND (LOWER(Email) = LOWER(@Email) OR LOWER(Username) = LOWER(@Username))";

                var duplicateCount = await connection.QuerySingleAsync<int>(duplicateCheckSql,
                    new { Id = id, Email = user.Email, Username = user.Username }, transaction);

                if (duplicateCount > 0)
                {
                    throw new InvalidOperationException("User with the same email or username already exists!");
                }

                // Validate role IDs if provided
                if (user.RoleIds != null && user.RoleIds.Length > 0)
                {
                    const string validateRolesSql = @"
                        SELECT COUNT(*) FROM Roles WHERE Id IN @RoleIds";

                    var validRoleCount = await connection.QuerySingleAsync<int>(validateRolesSql,
                        new { RoleIds = user.RoleIds }, transaction);

                    if (validRoleCount != user.RoleIds.Length)
                    {
                        throw new InvalidOperationException("Some provided role IDs are invalid.");
                    }
                }

                // Update user basic information
                const string updateUserSql = @"
                    UPDATE [Users] 
                    SET Name = @Name, 
                        Username = @Username, 
                        Email = @Email, 
                        IsActive = @IsActive,
                        UpdatedOn = @UpdatedOn
                    WHERE Id = @Id";

                var rowsAffected = await connection.ExecuteAsync(updateUserSql, new
                {
                    Id = id,
                    Name = user.Name,
                    Username = user.Username,
                    Email = user.Email,
                    IsActive = user.IsActive,
                    UpdatedOn = DateTime.UtcNow
                }, transaction);

                if (rowsAffected == 0)
                {
                    return false;
                }

                // Update roles if provided
                if (user.RoleIds != null)
                {
                    // Remove existing roles
                    const string deleteRolesSql = @"
                        DELETE FROM UserRole WHERE UserId = @UserId";

                    await connection.ExecuteAsync(deleteRolesSql, new { UserId = id }, transaction);

                    // Add new roles
                    if (user.RoleIds.Length > 0)
                    {
                        const string insertRolesSql = @"
                            INSERT INTO UserRole (UserId, RoleId) 
                            VALUES (@UserId, @RoleId)";

                        var userRoleData = user.RoleIds.Select(roleId => new
                        {
                            UserId = id,
                            RoleId = roleId
                        });

                        await connection.ExecuteAsync(insertRolesSql, userRoleData, transaction);
                    }
                }

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
    
}
