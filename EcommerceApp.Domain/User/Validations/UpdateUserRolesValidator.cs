using EcommerceApp.Domain.User.DTOs.Request;
using EcommerceApp.Domain.User.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Domain.User.Validations
{
    public class UpdateUserRolesValidator : AbstractValidator<UpdateUserRoles>
    {
        private readonly IUserRepository _userRepository;
        public UpdateUserRolesValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;

            RuleFor(x => x.Id)
                .MustAsync(async (id, cancellation) =>
                {
                    // Validate that the ID is a valid integer and exists in the database
                    return id > 0 && await _userRepository.UserExistsById(id);
                }).WithMessage("User does not exists")
                .NotEmpty()
                .WithMessage("Id is required");

            RuleFor(x => x.RoleIds)
                .MustAsync(async (roleIds, cancellation) =>
                {
                    // Validate that all role IDs exist
                    if (roleIds == null)
                        return true; // No roles to validate
                    if (!roleIds.Any())
                        return false;
                    return await _userRepository.RolesExistAsync(roleIds);
                }).WithMessage("One or more roles do not exist.");
           
            
        }
    }
}
