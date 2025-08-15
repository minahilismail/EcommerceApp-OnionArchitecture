using Dapper;
using EcommerceApp.Domain.User.DTOs.Request;
using EcommerceApp.Domain.User.DTOs.Response;
using EcommerceApp.Domain.User.Interfaces;
using EcommerceApp.Model.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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

        public async Task<bool> UserExistsById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = "SELECT COUNT(1) FROM [Users] WHERE Id = @Id";
            var count = await connection.QuerySingleAsync<int>(sql, new { Id = id });
            return count > 0;
        }

        public async Task<bool> UserExistsByEmail(string email, int? currentUserId)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = "SELECT COUNT(1) FROM [Users] WHERE LOWER(Email) = LOWER(@Email) AND Id != @CurrentUserId";
            var count = await connection.QuerySingleAsync<int>(sql, new { Email = email, CurrentUserId = currentUserId });
            return count > 0;
        }

        public async Task<bool> UserExistsByUsername(string username, int? currentUserId)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = "SELECT COUNT(1) FROM [Users] WHERE LOWER(Username) = LOWER(@Username) AND Id != @CurrentUserId";
            var count = await connection.QuerySingleAsync<int>(sql, new { Username = username, CurrentUserId = currentUserId });
            return count > 0;
        }

        public async Task<bool> RolesExistAsync(int[] roleIds)
        {
            // Validate role IDs if provided
            if (roleIds != null && roleIds.Length > 0)
            {
                const string validateRolesSql = @"
                        SELECT COUNT(*) FROM Roles WHERE Id IN @RoleIds";

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                //checking if user provided role ids actually exist in the database
                var count = await connection.ExecuteScalarAsync<int>(validateRolesSql, new { RoleIds = roleIds });

                return count == roleIds.Length;
            }

            return true;
        }
    }
    
}
