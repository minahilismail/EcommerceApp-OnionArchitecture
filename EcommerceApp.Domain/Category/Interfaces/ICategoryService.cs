
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
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponse>> GetAllAsync();
        Task<CategoryResponse?> GetByIdAsync(int id);
        Task<IEnumerable<CategoryResponse>> GetByStatusIdAsync(int statusId);
        Task<IEnumerable<CategoryResponse>> GetRootCategoriesAsync();
        Task<IEnumerable<CategoryStatusesDto>> GetStatuses();
        Task<PagedResult<CategoryResponse>> GetPagedAsync(PaginationParameters parameters, int? statusId = null);
        Task<CategoryResponse> CreateAsync(AddCategory createDto);
        Task<bool> UpdateAsync(int id, UpdateCategory updateDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateStatusAsync(int id, int statusId);
    }
}
