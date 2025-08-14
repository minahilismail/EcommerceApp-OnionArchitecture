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
        public Task<IEnumerable<UserResponse>> GetUsersAsync();
        public Task<UserResponse> UpdateUserRole(UpdateUserRoles updateUserRoles);
        public Task<IEnumerable<RoleModel>> GetRoles();
    }
}
