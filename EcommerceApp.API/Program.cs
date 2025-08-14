using EcommerceApp.API.Endpoints;
using EcommerceApp.Domain.Category.Interfaces;
using EcommerceApp.Domain.Category.Repository;
using EcommerceApp.Domain.Category.Service;
using EcommerceApp.Domain.Product.Interfaces;
using EcommerceApp.Domain.Product.Repository;
using EcommerceApp.Domain.Product.Service;
using EcommerceApp.Domain.User.Interfaces;
using EcommerceApp.Domain.User.Repository;
using EcommerceApp.Domain.User.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Registering services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};
CategoryEndpoints.MapCategoryEndpoints(app);
ProductEndpoints.MapProductEndpoints(app);
UserEndpoints.MapUserEndpoints(app);
app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
