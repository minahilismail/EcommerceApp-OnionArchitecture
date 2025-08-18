using EcommerceApp.Domain.User.DTOs.Request;
using EcommerceApp.Domain.User.DTOs.Response;
using EcommerceApp.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Domain.User.Interfaces
{
    public interface IUserRepository
    {
        public Task<IEnumerable<UserModel>> GetUsersAsync();
        public Task<UserModel> GetUserById(int id);
        public Task<bool> UpdateUserRole(UpdateUserRoles updateUserRoles);
        public Task<IEnumerable<RoleModel>> GetRoles();
        public Task<bool> UpdateUser(int id, UpdateUser user);
        public Task<bool> UserExistsById(int id);
        public Task<bool> UserExistsByEmail(string email, int? id);
        public Task<bool> UserExistsByUsername(string username, int? id);
        public Task<bool> RolesExistAsync(int[] roleIds);

    }
}
