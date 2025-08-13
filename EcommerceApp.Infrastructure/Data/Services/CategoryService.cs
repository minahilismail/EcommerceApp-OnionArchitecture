using EcommerceApp.Application.DTOs.Request;
using EcommerceApp.Application.DTOs.Response;
using EcommerceApp.Application.Extensions.Mappings;
using EcommerceApp.Application.Interfaces.Repositories;
using EcommerceApp.Application.Interfaces.Services;
using EcommerceApp.Domain.Category.Interfaces;
using EcommerceApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Infrastructure.Data.Services
{
    internal class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<CategoryResponse> CreateCategoryAsync(AddCategory createDto)
        {
            // Validate parent category exists if provided
            if (createDto.ParentCategoryId.HasValue)
            {
                var parentExists = await _categoryRepository.ExistsAsync(createDto.ParentCategoryId.Value);
                if (!parentExists)
                    throw new ArgumentException("Parent category does not exist.");
            }

            // Check for duplicate name
            var nameExists = await _categoryRepository.ExistsByNameAsync(createDto.Name);
            if (nameExists)
                throw new ArgumentException("Category with this name already exists.");

            // Check for duplicate code
            var codeExists = await _categoryRepository.ExistsByCodeAsync(createDto.Code);
            if (codeExists)
                throw new ArgumentException("Category with this code already exists.");

            var category = CategoryMapping.ToEntity(createDto);

            // Calculate level based on parent
            if (createDto.ParentCategoryId.HasValue)
            {
                var parent = await _categoryRepository.GetCategoryByIdAsync(createDto.ParentCategoryId.Value);
                category.Level = (parent?.Level ?? 0) + 1;
            }
            else
            {
                category.Level = 0;
            }

            var id = await _categoryRepository.CreateCategoryAsync(category);
            category.Id = id;

            return CategoryMapping.ToDto(category);
        }
    }
}
