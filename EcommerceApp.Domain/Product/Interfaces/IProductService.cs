using EcommerceApp.Application.DTOs.Request;
using EcommerceApp.Application.DTOs.Response;
using EcommerceApp.Domain.Product.DTOs.Request;
using EcommerceApp.Domain.Product.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Domain.Product.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponse>> GetAllAsync();
        Task<ProductResponse?> GetByIdAsync(int id);
        Task<IEnumerable<ProductResponse>> GetByCategoryAsync(int categoryId);
        Task<PagedResult<ProductResponse>> GetPagedAsync(PaginationParameters parameters, ProductFilter? filter = null);
        Task<ProductResponse> CreateAsync(CreateProduct createDto);
        Task<bool> UpdateAsync(int id, UpdateProduct updateDto);
        Task<bool> DeleteAsync(int id);
        Task<string> UploadImageAsync(UploadProductImage uploadDto);

    }
}
