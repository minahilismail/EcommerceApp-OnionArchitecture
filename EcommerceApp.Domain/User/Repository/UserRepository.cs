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

        public Task<UserResponse> UpdateUserRole(UpdateUserRoles updateUserRoles)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<UserResponse>> IUserRepository.GetUsersAsync()
        {
            throw new NotImplementedException();
        }
    }
}
