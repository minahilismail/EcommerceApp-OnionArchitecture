
using EcommerceApp.Application.DTOs.Request;
using EcommerceApp.Application.DTOs.Response;
using EcommerceApp.Domain.Category.DTOs.Request;
using EcommerceApp.Domain.Category.DTOs.Response;
using EcommerceApp.Domain.Category.Interfaces;
using EcommerceApp.Domain.Category.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Domain.Category.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<CategoryResponse> CreateAsync(AddCategory createDto)
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

            var category = CategoryExtension.ToEntity(createDto);

            // Calculate level based on parent
            if (createDto.ParentCategoryId.HasValue)
            {
                var parent = await _categoryRepository.GetByIdAsync(createDto.ParentCategoryId.Value);
                category.Level = (parent?.Level ?? 0) + 1;
            }
            else
            {
                category.Level = 0;
            }

            var id = await _categoryRepository.CreateAsync(category);
            category.Id = id;

            return CategoryExtension.ToDto(category);
        }


        public async Task<IEnumerable<CategoryResponse>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories.Select(Mappings.CategoryExtension.ToDto);
        }

        public async Task<CategoryResponse?> GetByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            return category != null ? Mappings.CategoryExtension.ToDto(category) : null;
        }

        public async Task<IEnumerable<CategoryResponse>> GetByStatusIdAsync(int statusId)
        {
            var categories = await _categoryRepository.GetByStatusIdAsync(statusId);
            return categories.Select(Mappings.CategoryExtension.ToDto);
        }

        public async Task<IEnumerable<CategoryResponse>> GetRootCategoriesAsync()
        {
            var categories = await _categoryRepository.GetRootCategoriesAsync();
            return categories.Select(Mappings.CategoryExtension.ToDto);
        }

        public async Task<PagedResult<CategoryResponse>> GetPagedAsync(PaginationParameters parameters, int? statusId = null)
        {
            var pagedResult = await _categoryRepository.GetPagedAsync(parameters, statusId);

            var categoryResponses = pagedResult.Data.Select(CategoryExtension.ToDto).ToList();

            return new PagedResult<CategoryResponse>(categoryResponses, pagedResult.TotalRecords, pagedResult.PageNumber, pagedResult.PageSize);
        }

        public async Task<bool> UpdateAsync(int id, UpdateCategory updateDto)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(id);
            if (existingCategory == null)
                return false;

            

            // Check for duplicate name (excluding current category)
            var nameExists = await _categoryRepository.ExistsByNameAsync(updateDto.Name, id);
            if (nameExists)
                throw new ArgumentException("Category with this name already exists.");

            // Check for duplicate code (excluding current category)
            var codeExists = await _categoryRepository.ExistsByCodeAsync(updateDto.Code, id);
            if (codeExists)
                throw new ArgumentException("Category with this code already exists.");

            existingCategory.UpdateFromDto(updateDto);
            return await _categoryRepository.UpdateAsync(existingCategory);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _categoryRepository.DeleteAsync(id);
        }

        public async Task<bool> UpdateStatusAsync(int id, int statusId)
        {
            return await _categoryRepository.UpdateCategoryStatusAsync(id, statusId);
        }
    }
}
