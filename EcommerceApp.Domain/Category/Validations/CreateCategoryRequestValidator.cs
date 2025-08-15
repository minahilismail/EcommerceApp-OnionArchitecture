using EcommerceApp.Domain.Category.DTOs.Request;
using EcommerceApp.Domain.Category.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Domain.Category.Validations
{
    public class CreateCategoryRequestValidator : AbstractValidator<AddCategory>
    {
        private readonly ICategoryRepository _categoryRepository;

        public CreateCategoryRequestValidator(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.")
                .MustAsync(async (name, cancellation) =>
                    !await _categoryRepository.ExistsByNameAsync(name))
                .WithMessage("Category with this name already exists.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required.")
                .MaximumLength(50).WithMessage("Code cannot exceed 50 characters.")
                .Matches("^[A-Z0-9_]+$").WithMessage("Code must contain only uppercase letters, numbers, and underscores.")
                .MustAsync(async (code, cancellation) =>
                    !await _categoryRepository.ExistsByCodeAsync(code))
                .MustAsync(async (name, cancellation)=>
                    !await _categoryRepository.ExistsByNameAsync(name))
                .WithMessage("Category with this code already exists.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.ParentCategoryId)
                .MustAsync(async (parentId, cancellation) =>
                {
                    if (!parentId.HasValue) return true;
                    return await _categoryRepository.ExistsAsync(parentId.Value);
                })
                .WithMessage("Parent category does not exist.")
                .When(x => x.ParentCategoryId.HasValue);
        }
    }
}
