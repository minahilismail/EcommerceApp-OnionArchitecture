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

        public Task<IEnumerable<UserResponse>> GetUsersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<UserResponse> UpdateUserRole(UpdateUserRoles updateUserRoles)
        {
            throw new NotImplementedException();
        }
    }
}
