using EcommerceApp.Domain.Auth.DTOs.Request;
using EcommerceApp.Domain.Auth.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Domain.Auth.Validations
{
    public class SignUpUserRequestValidator : AbstractValidator<SignupRequest>
    {
        private readonly IAuthRepository _authRepository;
        public SignUpUserRequestValidator(IAuthRepository authRepository)
        {
            _authRepository = authRepository;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.").MinimumLength(2).WithMessage("Name must be at least 2 characters long.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MustAsync(async (email, cancellation) => !await _authRepository.UserExistsAsync(email, null))
                .WithMessage("Email already exists.");
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
                .MaximumLength(50).WithMessage("Username must not exceed 50 characters.")
                .MustAsync(async (username, cancellation) => !await _authRepository.UserExistsAsync(null, username))
                .WithMessage("Username already exists.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
        }
    }
}