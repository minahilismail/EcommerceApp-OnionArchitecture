using EcommerceApp.Domain.User.DTOs.Request;
using EcommerceApp.Domain.User.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Domain.User.Interfaces
{
    public interface IUserService
    {
        public Task<IEnumerable<UserResponse>> GetUsersAsync();
        public Task<bool> UpdateUserRole(UpdateUserRoles updateUserRoles);
        public Task<IEnumerable<UserRoleDto>> GetRoles();
        public Task<bool> UpdateUser(int id, UpdateUser updateUserRequest);
    }
}
