
using EcommerceApp.Core.DTOs;
using EcommerceApp.Domain.Category.DTOs.Request;
using EcommerceApp.Domain.Category.DTOs.Response;
using EcommerceApp.Domain.Category.Interfaces;
using EcommerceApp.Domain.Category.Mappings;
using EcommerceApp.Model.Entities;
using FluentValidation;
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
        private readonly IValidator<UpdateCategory> _updateCategoryValidator;
        private readonly IValidator<AddCategory> _addCategoryValidator;

        public CategoryService(ICategoryRepository categoryRepository, IValidator<UpdateCategory> updateCategoryValidator, IValidator<AddCategory> addCategoryValidator)
        {
            _categoryRepository = categoryRepository;
            _updateCategoryValidator = updateCategoryValidator;
            _addCategoryValidator = addCategoryValidator;
        }
        public async Task<CategoryResponse> CreateAsync(AddCategory createDto)
        {
            // Validate the DTO using FluentValidation
            var validationResult = await _addCategoryValidator.ValidateAsync(createDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                throw new ArgumentException(string.Join(", ", errors));
            }

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
            return categories.Select(CategoryExtension.ToDto);
        }

        public async Task<CategoryResponse?> GetByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            return category != null ? CategoryExtension.ToDto(category) : null;
        }

        public async Task<IEnumerable<CategoryResponse>> GetByStatusIdAsync(int statusId)
        {
            var categories = await _categoryRepository.GetByStatusIdAsync(statusId);
            return categories.Select(CategoryExtension.ToDto);
        }

        public async Task<IEnumerable<CategoryResponse>> GetRootCategoriesAsync()
        {
            var categories = await _categoryRepository.GetRootCategoriesAsync();
            return categories.Select(CategoryExtension.ToDto);
        }

        public async Task<PagedResult<CategoryResponse>> GetPagedAsync(PaginationParameters parameters, int? statusId = null)
        {
            var pagedResult = await _categoryRepository.GetPagedAsync(parameters, statusId);

            var categoryResponses = pagedResult.Data.Select(CategoryExtension.ToDto).ToList();

            return new PagedResult<CategoryResponse>(categoryResponses, pagedResult.TotalRecords, pagedResult.PageNumber, pagedResult.PageSize);
        }

        public async Task<bool> UpdateAsync(int id, UpdateCategory updateDto)
        {
            // Validate the DTO using FluentValidation
            var validationResult = await _updateCategoryValidator.ValidateAsync(updateDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                throw new ArgumentException(string.Join(", ", errors));
            }
            var existingCategory = await _categoryRepository.GetByIdAsync(id);
            if (existingCategory == null)
                return false;
            
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

        public async Task<IEnumerable<CategoryStatusesDto>> GetStatuses()
        {
            return (await _categoryRepository.GetStatuses()).Select(CategoryExtension.ToDto);
        }
    }
}
