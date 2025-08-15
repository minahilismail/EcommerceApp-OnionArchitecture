using EcommerceApp.Domain.Product.DTOs.Request;
using EcommerceApp.Domain.Product.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Domain.Product.Validations
{
    public class UpdateProductRequestValidator : AbstractValidator<UpdateProduct>
    {
        private readonly IProductRepository _productRepository;
        public UpdateProductRequestValidator(IProductRepository productRepository)
        {
            _productRepository = productRepository;

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.");

            RuleFor(x => x.CategoryId)
                .MustAsync(async (categoryId, cancellation) =>
                    await _productRepository.CategoryExistsAsync(categoryId))
                .WithMessage("Category does not exist.");

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Product ID must be greater than 0.")
                .MustAsync(async (id, cancellation) =>
                    await _productRepository.ExistsByIdAsync(id))
                .WithMessage("Product does not exist.");

            RuleFor(x => x.Title)
                .MustAsync(async (model, title, cancellation) =>
                {
                    // Check for duplicate title (excluding current product)
                    return !await _productRepository.ExistsByTitleAsync(title, model.Id);
                })
                .WithMessage("Product with this title already exists.");

            RuleFor(x => x.Image)
                .Must(BeValidImageFile).WithMessage("Only image files are allowed (jpg, jpeg, png).")
                .When(x => x.Image != null);

            RuleFor(x => x.Image)
                .Must(BeValidFileSize).WithMessage("File size cannot exceed 5MB.")
                .When(x => x.Image != null);
        }

        private bool BeValidImageFile(IFormFile? file)
        {
            if (file == null) return true;
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return allowedExtensions.Contains(fileExtension);
        }

        private bool BeValidFileSize(IFormFile? file)
        {
            if (file == null) return true;
            return file.Length <= 5 * 1024 * 1024; // 5MB
        }

        
    }
}
