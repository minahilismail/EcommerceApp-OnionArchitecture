
using EcommerceApp.Core.DTOs;
using EcommerceApp.Domain.Category.DTOs.Request;
using EcommerceApp.Domain.Category.Interfaces;

namespace EcommerceApp.API.Endpoints
{
    public static class CategoryEndpoints
    {
        public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/categories")
                .WithTags("Categories")
                .WithOpenApi();
            group.MapGet("/", async (ICategoryService categoryService) =>
            {
                var categories = await categoryService.GetAllAsync();
                return Results.Ok(categories);
            }).WithSummary("Get all categories");
            group.MapGet("/{id:int}", async (int id, ICategoryService categoryService) =>
            {
                var category = await categoryService.GetByIdAsync(id);
                return category is not null ? Results.Ok(category) : Results.NotFound();
            }).WithSummary("Get category by id");
            group.MapPost("/", async (AddCategory createCategoryDto, ICategoryService categoryService) =>
            {
                var createdCategory = await categoryService.CreateAsync(createCategoryDto);
                return Results.Created($"/api/categories/{createdCategory.Id}", createdCategory);
            }).WithSummary("Create category").RequireAuthorization("AdminOnly");
            group.MapDelete(("/{id:int}"), async (int id, ICategoryService categoryService) =>
            {
                var deleted = await categoryService.DeleteAsync(id);
                return deleted ? Results.NoContent() : Results.NotFound();
            }).WithSummary("Delete Category").RequireAuthorization("AdminOnly");
            group.MapPut(("/{id:int}"), async (int id, UpdateCategory updateCategoryDto, ICategoryService categoryService) =>
            {
                var updated = await categoryService.UpdateAsync(id, updateCategoryDto);
                return updated ? Results.NoContent() : Results.NotFound();
            }).WithSummary("Update Category").RequireAuthorization("AdminOnly");
            group.MapPut("/{id:int}/status", async (int id, int statusId, ICategoryService categoryService) =>
            {
                var updated = await categoryService.UpdateStatusAsync(id, statusId);
                return updated ? Results.NoContent() : Results.NotFound();
            }).WithSummary("Update Category Status").RequireAuthorization("AdminOnly");
            group.MapGet("/status/{statusId:int}", async (int statusId, ICategoryService categoryService) =>
            {
                var categories = await categoryService.GetByStatusIdAsync(statusId);
                return Results.Ok(categories);
            }).WithSummary("Get Categories by Status");
            group.MapGet("/paged", async (
                [AsParameters] PaginationParameters paginationParameters,
                int? statusId,
                ICategoryService categoryService) =>
            {
                var pagedCategories = await categoryService.GetPagedAsync(paginationParameters, statusId);
                return Results.Ok(pagedCategories);
            }).WithSummary("Get Paged Categories");
            group.MapGet("/statuses", async (ICategoryService categoryService) =>
            {
                var statuses = await categoryService.GetStatuses();
                return Results.Ok(statuses);
            }).WithSummary("Get Category Statuses");

        }
    }
}