using EcommerceApp.Domain.Category.Interfaces;
using EcommerceApp.Domain.Product.DTOs.Request;
using EcommerceApp.Domain.User.DTOs.Request;
using EcommerceApp.Domain.User.DTOs.Response;
using EcommerceApp.Domain.User.Interfaces;
using EcommerceApp.Domain.User.Mappings;
using EcommerceApp.Domain.User.Validations;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Domain.User.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UpdateUserRequestValidator _updateUserValidator;
        private readonly IValidator<UpdateUserRoles> _updateUserRolesValidator;

        public UserService(IUserRepository userRepository, UpdateUserRequestValidator updateUserValidator, IValidator<UpdateUserRoles> updateUserRolesValidator)
        {
            _userRepository = userRepository;
            _updateUserValidator = updateUserValidator;
            _updateUserRolesValidator = updateUserRolesValidator;
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
            // Validate using FluentValidation with user ID
            var validationResult = await _updateUserValidator.ValidateWithUserIdAsync(updateUserRequest, id);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                throw new ArgumentException(string.Join(", ", errors));
            }
            return await _userRepository.UpdateUser(id, updateUserRequest);
        }

        public async Task<bool> UpdateUserRole(UpdateUserRoles updateUserRoles)
        {
            var valid = await _updateUserRolesValidator.ValidateAsync(updateUserRoles);
            if (!valid.IsValid) {
                var errors = valid.Errors.Select(e=>e.ErrorMessage).ToList();
                throw new ArgumentException(string.Join(", ", errors));
            }
            return await _userRepository.UpdateUserRole(updateUserRoles);
        }
    }
}
