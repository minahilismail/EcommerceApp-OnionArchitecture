using EcommerceApp.Domain.Product.DTOs.Request;
using EcommerceApp.Domain.Product.DTOs.Response;
using EcommerceApp.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Domain.Product.Mappings
{
    public static class ProductExtensions
    {
        // Domain to DTO mappings
        public static ProductResponse ToDto(this ProductModel product)
        {
            return new ProductResponse
            {
                Id = product.Id,
                Title = product.Title,
                Price = product.Price,
                Description = product.Description,
                Image = product.Image,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                
            };
        }

        public static IEnumerable<ProductResponse> ToDto(this IEnumerable<ProductModel> products)
        {
            return products.Select(p => p.ToDto());
        }

        // DTO to Domain mappings
        public static ProductModel ToEntity(this CreateProduct dto)
        {
            return new ProductModel
            {
                Title = dto.Title,
                Price = dto.Price,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                Image = null
            };
        }

        public static void UpdateFromDto(this ProductModel entity, UpdateProduct dto)
        {
            entity.Title = dto.Title;
            entity.Price = dto.Price;
            entity.Description = dto.Description;
            entity.CategoryId = dto.CategoryId;
        }
 
    }
}
