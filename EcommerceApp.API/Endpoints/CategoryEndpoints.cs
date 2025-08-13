using EcommerceApp.Application.DTOs.Request;
using EcommerceApp.Domain.Category.DTOs.Request;
using EcommerceApp.Domain.Category.Interfaces;

namespace EcommerceApp.API.Endpoints
{
    public class CategoryEndpoints
    {
        public CategoryEndpoints() { }
        public static void MapCategoryEndpoints(WebApplication app)
        {
            app.MapGet("/api/categories", async (ICategoryService categoryService) =>
            {
                var categories = await categoryService.GetAllAsync();
                return Results.Ok(categories);
            }).WithSummary("Get all categories");
            app.MapGet("/api/categories/{id:int}", async (int id, ICategoryService categoryService) =>
            {
                var category = await categoryService.GetByIdAsync(id);
                return category is not null ? Results.Ok(category) : Results.NotFound();
            }).WithSummary("Get category by id");
            app.MapPost("/api/categories", async (AddCategory createCategoryDto, ICategoryService categoryService) =>
            {
                var createdCategory = await categoryService.CreateAsync(createCategoryDto);
                return Results.Created($"/api/categories/{createdCategory.Id}", createdCategory);
            }).WithSummary("Create category");
            app.MapDelete(("/api/categories/{id:int}"), async (int id, ICategoryService categoryService) =>
            {
                var deleted = await categoryService.DeleteAsync(id);
                return deleted ? Results.NoContent() : Results.NotFound();
            }).WithSummary("Delete Category");
            app.MapPut(("/api/categories/{id:int}"), async (int id, UpdateCategory updateCategoryDto, ICategoryService categoryService) =>
            {
                var updated = await categoryService.UpdateAsync(id, updateCategoryDto);
                return updated ? Results.NoContent() : Results.NotFound();
            }).WithSummary("Update Category");
            app.MapPut("/api/categories/{id:int}/status", async (int id, int statusId, ICategoryService categoryService) =>
            {
                var updated = await categoryService.UpdateStatusAsync(id, statusId);
                return updated ? Results.NoContent() : Results.NotFound();
            }).WithSummary("Update Category Status");
            app.MapGet("/api/categories/status/{statusId:int}", async (int statusId, ICategoryService categoryService) =>
            {
                var categories = await categoryService.GetByStatusIdAsync(statusId);
                return Results.Ok(categories);
            }).WithSummary("Get Categories by Status");
            app.MapGet("/api/categories/paged", async (
                [AsParameters] PaginationParameters paginationParameters,
                int? statusId,
                ICategoryService categoryService) =>
            {
                var pagedCategories = await categoryService.GetPagedAsync(paginationParameters, statusId);
                return Results.Ok(pagedCategories);
            }).WithSummary("Get Paged Categories");

        }
    }
}