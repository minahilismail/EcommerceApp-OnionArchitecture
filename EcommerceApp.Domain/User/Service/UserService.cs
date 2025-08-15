using EcommerceApp.Domain.Category.Interfaces;
using EcommerceApp.Domain.User.DTOs.Request;
using EcommerceApp.Domain.User.DTOs.Response;
using EcommerceApp.Domain.User.Interfaces;
using EcommerceApp.Domain.User.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Domain.User.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserRoleDto>> GetRoles()
        {
            var roles = await _userRepository.GetRoles();
            return roles.ToRoleDto();
        }

        public async Task<IEnumerable<UserResponse>> GetUsersAsync()
        {
            var users = await _userRepository.GetUsersAsync();
            return users.ToDto();
        }

        public async Task<bool> UpdateUser(int id, UpdateUser updateUserRequest)
        {
            return await _userRepository.UpdateUser(id, updateUserRequest);
        }

        public Task<bool> UpdateUserRole(UpdateUserRoles updateUserRoles)
        {
            if (updateUserRoles == null || updateUserRoles.Id <= 0 || updateUserRoles.RoleIds == null || !updateUserRoles.RoleIds.Any())
            {
                throw new ArgumentException("Invalid user role update request.");
            }
            return _userRepository.UpdateUserRole(updateUserRoles);

        }
    }
}
