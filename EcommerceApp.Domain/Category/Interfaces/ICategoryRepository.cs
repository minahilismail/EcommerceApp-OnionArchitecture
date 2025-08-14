
using EcommerceApp.Core.DTOs;
using EcommerceApp.Domain.Category.DTOs.Request;
using EcommerceApp.Domain.Category.DTOs.Response;
using EcommerceApp.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Domain.Category.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<CategoryModel>> GetAllAsync();
        Task<CategoryModel?> GetByIdAsync(int id);
        Task<IEnumerable<CategoryModel>> GetByStatusIdAsync(int statusId);
        Task<IEnumerable<CategoryModel>> GetRootCategoriesAsync();
        Task<PagedResult<CategoryModel>> GetPagedAsync(PaginationParameters parameters, int? statusId = null);

        Task<int> CreateAsync(CategoryModel category);
        Task<bool> UpdateAsync(CategoryModel category);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateCategoryStatusAsync(int id, int statusId);

        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
        Task<bool> ExistsByCodeAsync(string code, int? excludeId = null);
    }
}
