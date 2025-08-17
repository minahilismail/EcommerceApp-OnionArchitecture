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
    public class UpdateUserRequestValidator : AbstractValidator<UpdateUser>
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserRequestValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(2).WithMessage("Name must be at least 2 characters long.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
                .MaximumLength(50).WithMessage("Username must not exceed 50 characters.")
                .MustAsync(async (model, username, context, cancellation) =>
                {
                    // Get the user ID from the validation context
                    if (context.RootContextData.TryGetValue("UserId", out var userIdObj) && userIdObj is int userId)
                    {
                        // Check for duplicate username (excluding current user)
                        
                        return !await _userRepository.UserExistsByUsername(username, userId);
                    }
                    return false; // If no UserId is provided, validation fails

                }).WithMessage("Username already exists.");

            RuleFor(x => x.Email)
                .MustAsync(async (model, email, context, cancellation) =>
                {
                    // Get the user ID from the validation context
                    if (context.RootContextData.TryGetValue("UserId", out var userIdObj) && userIdObj is int userId)
                    {
                        // Check for duplicate email (excluding current user)
                        return !await _userRepository.UserExistsByEmail(email, userId);
                    }
                    return false; // If no UserId is provided, validation fails
                }).WithMessage("Email already exists.");
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

        // Method to validate with user ID
        public async Task<FluentValidation.Results.ValidationResult> ValidateWithUserIdAsync(UpdateUser instance, int userId)
        {
            var context = new ValidationContext<UpdateUser>(instance);
            context.RootContextData["UserId"] = userId;

            // Validate user ID exists
            if (userId <= 0)
            {
                var result = new FluentValidation.Results.ValidationResult();
                result.Errors.Add(new FluentValidation.Results.ValidationFailure("UserId", "User ID must be greater than 0."));
                return result;
            }

            var userExists = await _userRepository.UserExistsById(userId);
            if (!userExists)
            {
                var result = new FluentValidation.Results.ValidationResult();
                result.Errors.Add(new FluentValidation.Results.ValidationFailure("UserId", "User does not exist."));
                return result;
            }

            return await ValidateAsync(context);
        }
    }
}
