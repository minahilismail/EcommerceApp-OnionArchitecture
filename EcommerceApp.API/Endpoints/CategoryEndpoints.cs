namespace EcommerceApp.API.Endpoints
{
    public class CategoryEndpoints
    {
        public CategoryEndpoints() { }
        public static void MapCategoryEndpoints(WebApplication app)
        {
            app.MapGet("/api/categories", async (ICategoryService categoryService) =>
            {
                var categories = await categoryService.GetAllCategoriesAsync();
                return Results.Ok(categories);
            });
            app.MapGet("/api/categories/{id:int}", async (int id, ICategoryService categoryService) =>
            {
                var category = await categoryService.GetCategoryByIdAsync(id);
                return category is not null ? Results.Ok(category) : Results.NotFound();
            });
            app.MapPost("/api/categories", async (CreateCategoryDto createCategoryDto, ICategoryService categoryService) =>
            {
                var createdCategory = await categoryService.CreateCategoryAsync(createCategoryDto);
                return Results.Created($"/api/categories/{createdCategory.Id}", created
    }
}
