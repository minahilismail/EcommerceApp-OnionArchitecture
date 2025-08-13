using Dapper;
using EcommerceApp.Application.DTOs.Request;
using EcommerceApp.Application.DTOs.Response;
using EcommerceApp.Application.Interfaces.Repositories;
using EcommerceApp.Domain.Category.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Infrastructure.Data.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly string _connectionString;

        public CategoryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultSQLConnection")!;
        }
        public async Task<int> CreateCategoryAsync(AddCategory category)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                INSERT INTO Categories (Name, Code, Description, Level, ParentCategoryId, StatusId, CreatedDate, UpdatedDate)
                VALUES (@Name, @Code, @Description, @Level, @ParentCategoryId, @StatusId, @CreatedDate, @UpdatedDate);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            return await connection.QuerySingleAsync<int>(sql, category);
        }

        //public Task<bool> DeleteCategoryAsync(int id)
        //{
            
        //}

        //public Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync()
        //{
            
        //}

        //public Task<IEnumerable<CategoryResponse>> GetCategoriesByStatusIdAsync(int statusId)
        //{
            
        //}

        //public Task<CategoryResponse?> GetCategoryByIdAsync(int id)
        //{
            
        //}

        //public Task<PagedResult<CategoryResponse>> GetPagedCategoriesAsync(PaginationParameters parameters, int? statusId = null)
        //{
            
        //}

        //public Task<IEnumerable<CategoryResponse>> GetRootCategoriesAsync()
        //{
            
        //}

        //public Task<bool> UpdateCategoryAsync(UpdateCategory category)
        //{
            
        //}

        //public Task<bool> UpdateCategoryStatusAsync(int id, int statusId)
        //{
            
        //}
    }
}
