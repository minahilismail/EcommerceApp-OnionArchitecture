using EcommerceApp.Core.DTOs;
using EcommerceApp.Domain.Product.DTOs.Request;
using EcommerceApp.Domain.Product.DTOs.Response;
using EcommerceApp.Domain.Product.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace EcommerceApp.API.Endpoints
{
    public static class ProductEndpoints
    {
        public static void MapProductEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/products")
                .WithTags("Products")
                .WithOpenApi();

            // GET /api/products
            group.MapGet("/", async (IProductService productService) =>
            {
                var products = await productService.GetAllAsync();
                return Results.Ok(products);
            })
            .WithName("GetAllProducts")
            .WithSummary("Get all products")
            .Produces<IEnumerable<ProductResponse>>();

            // GET /api/products/paged
            group.MapGet("/paged", async (
                [AsParameters] PaginationParameters parameters,
                int? categoryId,
                string? searchTerm,
                double? minPrice,
                double? maxPrice,
                IProductService productService) =>
            {
                var filter = new ProductFilter
                {
                    CategoryId = categoryId,
                    SearchTerm = searchTerm,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice
                };

                var result = await productService.GetPagedAsync(parameters, filter);
                return Results.Ok(result);
            })
            .WithName("GetPagedProducts")
            .WithSummary("Get paginated products")
            .Produces<PagedResult<ProductResponse>>();

            // GET /api/products/{id}
            group.MapGet("/{id:int}", async (int id, IProductService productService) =>
            {
                if (id <= 0)
                    return Results.BadRequest("Product ID must be greater than 0.");

                var product = await productService.GetByIdAsync(id);
                return product != null ? Results.Ok(product) : Results.NotFound($"Product with ID {id} not found.");
            })
            .WithName("GetProductById")
            .WithSummary("Get product by ID")
            .Produces<ProductResponse>()
            .Produces(404);

            // GET /api/products/category/{categoryId}
            group.MapGet("/category/{categoryId:int}", async (int categoryId, IProductService productService) =>
            {
                if (categoryId <= 0)
                    return Results.BadRequest("Category ID must be greater than 0.");

                var products = await productService.GetByCategoryAsync(categoryId);
                return Results.Ok(products);
            })
            .WithName("GetProductsByCategory")
            .WithSummary("Get products by category")
            .Produces<IEnumerable<ProductResponse>>();

            // POST /api/products (without image - same pattern as categories)
            group.MapPost("/", async (CreateProduct createDto, IProductService productService) =>
            {
                try
                {
                    var product = await productService.CreateAsync(createDto);
                    return Results.Created($"/api/products/{product.Id}", product);
                }
                catch (ValidationException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .WithName("CreateProduct")
            .WithSummary("Create a new product")
            .Produces<ProductResponse>(201)
            .Produces(400);

            // PUT /api/products/{id} (with form data support for images)
            group.MapPut("/{id:int}", async (int id, HttpRequest request, IProductService productService) =>
            {
                if (id <= 0)
                    return Results.BadRequest("Product ID must be greater than 0.");

                try
                {
                    var form = await request.ReadFormAsync();
                    var formKeys = string.Join(", ", form.Keys);

                    // Check if required fields exist
                    if (!form.ContainsKey("title") || string.IsNullOrEmpty(form["title"]))
                        return Results.BadRequest("Title is required");

                    if (!form.ContainsKey("price") || string.IsNullOrEmpty(form["price"]))
                        return Results.BadRequest("Price is required");

                    if (!form.ContainsKey("description") || string.IsNullOrEmpty(form["description"]))
                        return Results.BadRequest("Description is required");

                    if (!form.ContainsKey("categoryId") || string.IsNullOrEmpty(form["categoryId"]))
                        return Results.BadRequest("CategoryId is required");

                    // Parse form data with error handling
                    if (!double.TryParse(form["price"], out double price))
                        return Results.BadRequest("Invalid price format");

                    if (!int.TryParse(form["categoryId"], out int categoryId))
                        return Results.BadRequest("Invalid categoryId format");

                    var updateDto = new UpdateProduct
                    {
                        Id = id, 
                        Title = form["title"]!,
                        Price = price,
                        Description = form["description"]!,
                        CategoryId = categoryId,
                        Image = form.Files.GetFile("image") 
                    };

                    var success = await productService.UpdateAsync(id, updateDto);
                    return success ? Results.NoContent() : Results.NotFound($"Product with ID {id} not found.");
                }
                catch (FormatException ex)
                {
                    return Results.BadRequest($"Invalid data format: {ex.Message}");
                }
                catch (ValidationException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest($"Error processing request: {ex.Message}");
                }
            })
            .WithName("UpdateProduct")
            .WithSummary("Update an existing product")
            .Produces(204)
            .Produces(400)
            .Produces(404)
            .DisableAntiforgery(); // Required for file uploads

            // DELETE /api/products/{id}
            group.MapDelete("/{id:int}", async (int id, IProductService productService) =>
            {
                if (id <= 0)
                    return Results.BadRequest("Product ID must be greater than 0.");

                var success = await productService.DeleteAsync(id);
                return success ? Results.NoContent() : Results.NotFound($"Product with ID {id} not found.");
            })
            .WithName("DeleteProduct")
            .WithSummary("Delete a product")
            .Produces(204)
            .Produces(404);

            // POST /api/products/upload-image (separate endpoint for file uploads)
            group.MapPost("/upload-image", async (HttpRequest request, IProductService productService) =>
            {
                try
                {
                    var form = await request.ReadFormAsync();

                    var uploadDto = new UploadProductImage
                    {
                        ProductId = int.Parse(form["ProductId"]!),
                        Image = form.Files["Image"]!
                    };

                    var imageUrl = await productService.UploadImageAsync(uploadDto);
                    return Results.Ok(new { imageUrl });
                }
                catch (ValidationException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .WithName("UploadProductImage")
            .WithSummary("Upload product image")
            .Produces<object>(200)
            .Produces(400)
            .Produces(404)
            .DisableAntiforgery(); // For file upload
        }
    }
}
