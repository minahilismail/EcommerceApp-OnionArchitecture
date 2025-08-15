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
    public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategory>
    {
        private readonly ICategoryRepository _categoryRepository;

        public UpdateCategoryRequestValidator(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Category ID must be greater than 0.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.")
                .MustAsync(async (model, name, cancellation) =>
                {
                    // Check for duplicate name (excluding current category)
                    return !await _categoryRepository.ExistsByNameAsync(name, model.Id);
                })
                .WithMessage("Category with this name already exists.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required.")
                .MaximumLength(50).WithMessage("Code cannot exceed 50 characters.")
                .Matches("^[A-Z0-9_]+$").WithMessage("Code must contain only uppercase letters, numbers, and underscores.")
                .MustAsync(async (model, code, cancellation) =>
                {
                    // Check for duplicate code (excluding current category)
                    return !await _categoryRepository.ExistsByCodeAsync(code, model.Id);
                })
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
                .When(x => x.ParentCategoryId.HasValue)
                .MustAsync(async (model, parentId, cancellation) =>
                {
                    // Prevent self-reference
                    if (!parentId.HasValue) return true;
                    return parentId.Value != model.Id;
                })
                .WithMessage("Category cannot be its own parent.")
                .When(x => x.ParentCategoryId.HasValue);

            RuleFor(x => x.StatusId)
                .GreaterThan(0).WithMessage("Status ID must be greater than 0.")
                .When(x => x.StatusId.HasValue);
        }

    }
}
