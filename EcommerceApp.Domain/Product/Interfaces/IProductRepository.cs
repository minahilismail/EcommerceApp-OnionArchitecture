using EcommerceApp.Core.DTOs;
using EcommerceApp.Domain.Product.DTOs.Request;
using EcommerceApp.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Domain.Product.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductModel>> GetAllAsync();
        Task<ProductModel?> GetByIdAsync(int id);
        Task<IEnumerable<ProductModel>> GetByCategoryAsync(int categoryId);
        Task<PagedResult<ProductModel>> GetPagedAsync(PaginationParameters parameters, ProductFilter? filter = null);
        Task<bool> ExistsByTitleAsync(string title, int? excludeId = null);
        Task<bool> CategoryExistsAsync(int categoryId);
        Task<int> CreateAsync(ProductModel product);
        Task<bool> UpdateAsync(ProductModel product);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateImageAsync(int id, string imageUrl);
    }
}
