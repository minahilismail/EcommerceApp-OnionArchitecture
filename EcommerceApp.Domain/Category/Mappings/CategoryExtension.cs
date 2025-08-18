using EcommerceApp.Domain.Category.DTOs.Request;
using EcommerceApp.Domain.Category.DTOs.Response;
using EcommerceApp.Model.Entities;
using EcommerceApp.Model.Entities.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Domain.Category.Mappings
{
    public static class CategoryExtension
    {
        public static CategoryResponse ToCreateDto(this CreateUpdateCategoryModel category)
        {
            return new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Code = category.Code,
                Description = category.Description,
                Level = category.Level,
                ParentCategoryId = category.ParentCategoryId,
                StatusId = category.StatusId
            };
        }

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

                SubCategories = category.SubCategories?.Select(sub => new CategoryResponse
                {
                    Id = sub.Id,
                    Name = sub.Name,
                    Code = sub.Code,
                    Description = sub.Description,
                    Level = sub.Level,
                    ParentCategoryId = sub.ParentCategoryId,
                    StatusId = sub.StatusId,

                }).ToList() ?? new List<CategoryResponse>(),
            };
        }

        // DTO to Domain mappings
        public static CreateUpdateCategoryModel ToEntity(this AddCategory dto)
        {
            return new CreateUpdateCategoryModel
            {
                Name = dto.Name,
                Code = dto.Code,
                Description = dto.Description,
                ParentCategoryId = dto.ParentCategoryId,
                StatusId = 1,
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
            };
        }

        public static CreateUpdateCategoryModel ToModel(this CategoryModel model)
        {
            return new CreateUpdateCategoryModel
            {
                Id = model.Id,
                Name = model.Name,
                Code = model.Code,
                Description = model.Description,
                StatusId = model.StatusId,
            };
        }

        public static void UpdateFromDto(this CategoryModel entity, UpdateCategory dto)
        {
            entity.Name = dto.Name;
            entity.Code = dto.Code;
            entity.Description = dto.Description;
            entity.StatusId = dto.StatusId;
        }

        public static CategoryStatusesDto ToDto(this StatusModel status)
        {
            return new CategoryStatusesDto
            {
                Id = status.Id,
                Name = status.Name
            };
        }

              

    }
}
