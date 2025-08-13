using EcommerceApp.Domain.Category.DTOs.Request;
using EcommerceApp.Domain.Category.DTOs.Response;
using EcommerceApp.Model.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Domain.Category.Mappings
{
    public static class CategoryExtension
    {
        public static CategoryResponse ToDto(this CategoryModel category)
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
                SubCategories = category.SubCategories?.Select(sub => new CategoryResponse
                {
                    Id = sub.Id,
                    Name = sub.Name,
                    Code = sub.Code,
                    Description = sub.Description,
                    Level = sub.Level,
                    ParentCategoryId = sub.ParentCategoryId,
                    StatusId = sub.StatusId,
                    ParentCategoryName = sub.ParentCategory?.Name
                }).ToList() ?? new List<CategoryResponse>(),
            };
        }

        // DTO to Domain mappings
        public static CategoryModel ToEntity(this AddCategory dto)
        {
            return new CategoryModel
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

        public static CategoryModel ToEntity(this UpdateCategory dto)
        {
            return new CategoryModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Code = dto.Code,
                Description = dto.Description,
                StatusId = dto.StatusId,
                UpdatedDate = DateTime.UtcNow,
            };
        }

        public static void UpdateFromDto(this CategoryModel entity, UpdateCategory dto)
        {
            entity.Name = dto.Name;
            entity.Code = dto.Code;
            entity.Description = dto.Description;
            entity.StatusId = dto.StatusId;
            entity.UpdatedDate = DateTime.UtcNow;
        }
    }
}
