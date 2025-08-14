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
                FROM Role r 
                ORDER BY r.Id";

            return await connection.QueryAsync<RoleModel>(sql);
        }

        public async Task<bool> UpdateUserRole(UpdateUserRoles updateUserRoles)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
                UPDATE [User]
                SET UserRoles = @RoleIds
                WHERE Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, new { RoleIds = updateUserRoles.RoleIds, Id = updateUserRoles.Id });
            return rowsAffected > 0;
            
        }

        public async Task<IEnumerable<UserModel>> GetUsersAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
                SELECT u.Id, u.Name, u.Email, u.UserRoles, r.Name as RoleName
                FROM [User] u
                LEFT JOIN UserRole ur ON u.Id = ur.UserId
                LEFT JOIN Role r ON ur.RoleId = r.Id
                ORDER BY u.Id";

            return await connection.QueryAsync<UserModel>(sql);
        }
    }
}
