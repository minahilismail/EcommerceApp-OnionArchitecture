using EcommerceApp.Domain.Auth.DTOs.Request;
using EcommerceApp.Domain.Auth.DTOs.Response;
using EcommerceApp.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Domain.Auth.Interfaces
{
    public interface IAuthRepository
    {
        public Task<UserModel?> GetUserByEmailAsync(string email);
        public Task<int> CreateUserAsync(UserModel user);
        public Task<bool> UserExistsAsync(string email, string username);
        public Task<bool> AssignDefaultRoleAsync(int userId);
    }
}
