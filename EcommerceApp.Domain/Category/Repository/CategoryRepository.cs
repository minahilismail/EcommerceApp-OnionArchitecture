using Dapper;
using EcommerceApp.Core.DTOs;
using EcommerceApp.Domain.Category.DTOs.Request;
using EcommerceApp.Domain.Category.DTOs.Response;
using EcommerceApp.Domain.Category.Interfaces;
using EcommerceApp.Model.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Domain.Category.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly string _connectionString;

        public CategoryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultSQLConnection")!;
        }
        public async Task<int> CreateAsync(CategoryModel category)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                INSERT INTO Categories (Name, Code, Description, Level, ParentCategoryId, StatusId, CreatedDate, UpdatedDate)
                VALUES (@Name, @Code, @Description, @Level, @ParentCategoryId, @StatusId, @CreatedDate, @UpdatedDate);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            return await connection.QuerySingleAsync<int>(sql, category);
        }
        public async Task<IEnumerable<CategoryModel>> GetAllAsync()
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT c.*, p.Name as ParentCategoryName
                FROM Categories c
                LEFT JOIN Categories p ON c.ParentCategoryId = p.Id
                ORDER BY c.Level, c.Name";

            var categories = await connection.QueryAsync<CategoryModel>(sql);
            
            // Build the hierarchy
            var categoryList = categories.ToList();
            var categoryDict = categoryList.ToDictionary(c => c.Id);
            
            foreach (var category in categoryList)
            {
                if (category.ParentCategoryId.HasValue && categoryDict.ContainsKey(category.ParentCategoryId.Value))
                {
                    var parent = categoryDict[category.ParentCategoryId.Value];
                    parent.SubCategories.Add(category);
                }
            }
            
            return categoryList;
        }

        public async Task<CategoryModel?> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT c.*, p.Name as ParentCategoryName
                FROM Categories c
                LEFT JOIN Categories p ON c.ParentCategoryId = p.Id
                WHERE c.Id = @Id";

            return await connection.QueryFirstOrDefaultAsync<CategoryModel>(sql, new { Id = id });
        }

        public async Task<CategoryModel?> GetByCodeAsync(string code)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = "SELECT * FROM Categories WHERE Code = @Code";

            return await connection.QueryFirstOrDefaultAsync<CategoryModel>(sql, new { Code = code });
        }

        public async Task<IEnumerable<CategoryModel>> GetByStatusIdAsync(int statusId)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT c.*, p.Name as ParentCategoryName
                FROM Categories c
                LEFT JOIN Categories p ON c.ParentCategoryId = p.Id
                WHERE c.StatusId = @StatusId
                ORDER BY c.Level, c.Name";

            return await connection.QueryAsync<CategoryModel>(sql, new { StatusId = statusId });
        }

        public async Task<IEnumerable<CategoryModel>> GetRootCategoriesAsync()
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT * FROM Categories 
                WHERE ParentCategoryId IS NULL 
                ORDER BY Id";

            return await connection.QueryAsync<CategoryModel>(sql);
        }


        public async Task<PagedResult<CategoryModel>> GetPagedAsync(PaginationParameters parameters, int? statusId = null)
        {
            using var connection = new SqlConnection(_connectionString);

            var whereClause = "WHERE 1=1";
            var queryParams = new DynamicParameters();

            if (statusId.HasValue)
            {
                whereClause += " AND c.StatusId = @StatusId";
                queryParams.Add("StatusId", statusId.Value);
            }

            // Count query
            var countSql = $@"
                SELECT COUNT(*)
                FROM Categories c
                {whereClause}";

            var totalRecords = await connection.QuerySingleAsync<int>(countSql, queryParams);

            // Data query
            var offset = (parameters.PageNumber - 1) * parameters.PageSize;
            queryParams.Add("Offset", offset);
            queryParams.Add("PageSize", parameters.PageSize);

            var dataSql = $@"
                SELECT c.*, p.Name as ParentCategoryName
                FROM Categories c
                LEFT JOIN Categories p ON c.ParentCategoryId = p.Id
                {whereClause}
                ORDER BY c.Level, c.Name
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY";

            var data = await connection.QueryAsync<CategoryModel>(dataSql, queryParams);

            return new PagedResult<CategoryModel>(data.ToList(), totalRecords, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = "SELECT COUNT(1) FROM Categories WHERE Id = @Id";

            var count = await connection.QuerySingleAsync<int>(sql, new { Id = id });
            return count > 0;
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            using var connection = new SqlConnection(_connectionString);

            var sql = "SELECT COUNT(1) FROM Categories WHERE LOWER(Name) = LOWER(@Name)";
            var parameters = new DynamicParameters();
            parameters.Add("Name", name);

            if (excludeId.HasValue)
            {
                sql += " AND Id != @ExcludeId";
                parameters.Add("ExcludeId", excludeId.Value);
            }

            var count = await connection.QuerySingleAsync<int>(sql, parameters);
            return count > 0;
        }

        public async Task<bool> ExistsByCodeAsync(string code, int? excludeId = null)
        {
            using var connection = new SqlConnection(_connectionString);

            var sql = "SELECT COUNT(1) FROM Categories WHERE LOWER(Code) = LOWER(@Code)";
            var parameters = new DynamicParameters();
            parameters.Add("Code", code);

            if (excludeId.HasValue)
            {
                sql += " AND Id != @ExcludeId";
                parameters.Add("ExcludeId", excludeId.Value);
            }

            var count = await connection.QuerySingleAsync<int>(sql, parameters);
            return count > 0;
        }

      

        public async Task<bool> UpdateAsync(CategoryModel category)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                UPDATE Categories 
                SET Name = @Name, Code = @Code, Description = @Description, 
                    ParentCategoryId = @ParentCategoryId, StatusId = @StatusId, 
                    UpdatedDate = @UpdatedDate, UpdatedOn = @UpdatedOn, UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

            var rowsAffected = await connection.ExecuteAsync(sql, category);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = "DELETE FROM Categories WHERE Id = @Id";

            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }

        public async Task<bool> UpdateCategoryStatusAsync(int id, int statusId)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                UPDATE Categories 
                SET StatusId = @StatusId, UpdatedDate = @UpdatedDate, UpdatedOn = @UpdatedOn
                WHERE Id = @Id";

            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                Id = id,
                StatusId = statusId,
                UpdatedDate = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow
            });
            return rowsAffected > 0;
        }


    }
}
