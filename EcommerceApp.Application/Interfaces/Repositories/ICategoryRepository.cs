using EcommerceApp.Application.DTOs.Request;
using EcommerceApp.Application.DTOs.Response;
using EcommerceApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Application.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        //Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync();
        //Task<CategoryResponse?> GetCategoryByIdAsync(int id);
        //Task<IEnumerable<CategoryResponse>> GetCategoriesByStatusIdAsync(int statusId);
        //Task<IEnumerable<CategoryResponse>> GetRootCategoriesAsync();
        //Task<PagedResult<CategoryResponse>> GetPagedCategoriesAsync(PaginationParameters parameters, int? statusId = null);

        Task<int> CreateCategoryAsync(AddCategory category);
        //Task<bool> UpdateCategoryAsync(UpdateCategory category);
        //Task<bool> DeleteCategoryAsync(int id);
        //Task<bool> UpdateCategoryStatusAsync(int id, int statusId);
    }
}
