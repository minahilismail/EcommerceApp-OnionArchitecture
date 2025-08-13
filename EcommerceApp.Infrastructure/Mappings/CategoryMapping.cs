using EcommerceApp.Application.DTOs.Request;
using EcommerceApp.Application.DTOs.Response;
using EcommerceApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Application.Extensions.Mappings
{
    public static class CategoryMapping
    {
        public static CategoryResponse ToDto(this Category category)
        {
            return new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Code = category.Code,
                Description = category.Description,
                Level = category.Level,
                ParentCategoryId = category.ParentCategoryId,
                StatusId = category.StatusId,
                ParentCategoryName = category.ParentCategory?.Name,
                SubCategories = category.SubCategories?.Select(sub => new CategoryDTO
                {
                    Id = sub.Id,
                    Name = sub.Name,
                    Code = sub.Code,
                    Description = sub.Description,
                    Level = sub.Level,
                    ParentCategoryId = sub.ParentCategoryId,
                    StatusId = sub.StatusId,
                    ParentCategoryName = sub.ParentCategory?.Name
                }).ToList() ?? new List<CategoryDTO>(),
            };
        }

        // DTO to Domain mappings
        public static Category ToEntity(this AddCategory dto)
        {
            return new Category
            {
                Name = dto.Name,
                Code = dto.Code,
                Description = dto.Description,
                ParentCategoryId = dto.ParentCategoryId,
                StatusId = 1,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
                Level = 0 
            };
        }

        public static Category ToEntity(this UpdateCategory dto)
        {
            return new Category
            {
                Id = dto.Id,
                Name = dto.Name,
                Code = dto.Code,
                Description = dto.Description,
                StatusId = dto.StatusId,
                UpdatedDate = DateTime.UtcNow,
            };
        }

        
    }
}
