using Dapper;
using EcommerceApp.Application.DTOs.Request;
using EcommerceApp.Application.DTOs.Response;
using EcommerceApp.Domain.Product.DTOs.Request;
using EcommerceApp.Domain.Product.Interfaces;
using EcommerceApp.Model.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Domain.Product.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultSQLConnection")!;
        }

        public async Task<IEnumerable<ProductModel>> GetAllAsync()
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT p.*, c.Name as CategoryName
                FROM Products p
                LEFT JOIN Categories c ON p.CategoryId = c.Id
                ORDER BY p.Title";

            return await connection.QueryAsync<ProductModel>(sql);
        }

        public async Task<ProductModel?> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT p.*, c.Name as CategoryName
                FROM Products p
                LEFT JOIN Categories c ON p.CategoryId = c.Id
                WHERE p.Id = @Id";

            return await connection.QueryFirstOrDefaultAsync<ProductModel>(sql, new { Id = id });
        }

        public async Task<IEnumerable<ProductModel>> GetByCategoryAsync(int categoryId)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT p.*, c.Name as CategoryName
                FROM Products p
                LEFT JOIN Categories c ON p.CategoryId = c.Id
                WHERE p.CategoryId = @CategoryId
                ORDER BY p.Title";

            return await connection.QueryAsync<ProductModel>(sql, new { CategoryId = categoryId });
        }

        public async Task<PagedResult<ProductModel>> GetPagedAsync(PaginationParameters parameters, ProductFilter? filter = null)
        {
            using var connection = new SqlConnection(_connectionString);

            var whereClause = "WHERE 1=1";
            var queryParams = new DynamicParameters();

            if (filter?.CategoryId.HasValue == true)
            {
                whereClause += " AND p.CategoryId = @CategoryId";
                queryParams.Add("CategoryId", filter.CategoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter?.SearchTerm))
            {
                whereClause += " AND (p.Title LIKE @SearchTerm OR p.Description LIKE @SearchTerm)";
                queryParams.Add("SearchTerm", $"%{filter.SearchTerm}%");
            }

            if (filter?.MinPrice.HasValue == true)
            {
                whereClause += " AND p.Price >= @MinPrice";
                queryParams.Add("MinPrice", filter.MinPrice.Value);
            }

            if (filter?.MaxPrice.HasValue == true)
            {
                whereClause += " AND p.Price <= @MaxPrice";
                queryParams.Add("MaxPrice", filter.MaxPrice.Value);
            }

            // Count query
            var countSql = $@"
                SELECT COUNT(*)
                FROM Products p
                {whereClause}";

            var totalRecords = await connection.QuerySingleAsync<int>(countSql, queryParams);

            // Data query
            var offset = (parameters.PageNumber - 1) * parameters.PageSize;
            queryParams.Add("Offset", offset);
            queryParams.Add("PageSize", parameters.PageSize);

            var dataSql = $@"
                SELECT p.*, c.Name as CategoryName
                FROM Products p
                LEFT JOIN Categories c ON p.CategoryId = c.Id
                {whereClause}
                ORDER BY p.Title
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY";

            var data = await connection.QueryAsync<ProductModel>(dataSql, queryParams);

            return new PagedResult<ProductModel>(data.ToList(), totalRecords, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<bool> ExistsByTitleAsync(string title, int? excludeId = null)
        {
            using var connection = new SqlConnection(_connectionString);

            var sql = "SELECT COUNT(1) FROM Products WHERE LOWER(Title) = LOWER(@Title)";
            var parameters = new DynamicParameters();
            parameters.Add("Title", title);

            if (excludeId.HasValue)
            {
                sql += " AND Id != @ExcludeId";
                parameters.Add("ExcludeId", excludeId.Value);
            }

            var count = await connection.QuerySingleAsync<int>(sql, parameters);
            return count > 0;
        }

        public async Task<bool> CategoryExistsAsync(int categoryId)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = "SELECT COUNT(1) FROM Categories WHERE Id = @CategoryId";

            var count = await connection.QuerySingleAsync<int>(sql, new { CategoryId = categoryId });
            return count > 0;
        }

        public async Task<int> CreateAsync(ProductModel product)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                INSERT INTO Products (Title, Price, Description, Image, CategoryId, CreatedOn, CreatedBy, UpdatedOn, UpdatedBy)
                VALUES (@Title, @Price, @Description, @Image, @CategoryId, @CreatedOn, @CreatedBy, @UpdatedOn, @UpdatedBy);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            return await connection.QuerySingleAsync<int>(sql, product);
        }

        public async Task<bool> UpdateAsync(ProductModel product)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                UPDATE Products 
                SET Title = @Title, Price = @Price, Description = @Description, 
                    Image = @Image, CategoryId = @CategoryId, 
                    UpdatedOn = @UpdatedOn, UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

            var rowsAffected = await connection.ExecuteAsync(sql, product);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = "DELETE FROM Products WHERE Id = @Id";

            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }

        public async Task<bool> UpdateImageAsync(int id, string imageUrl)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                UPDATE Products 
                SET Image = @ImageUrl, UpdatedOn = @UpdatedOn
                WHERE Id = @Id";

            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                Id = id,
                ImageUrl = imageUrl,
                UpdatedOn = DateTime.UtcNow
            });
            return rowsAffected > 0;
        }
    }
}
