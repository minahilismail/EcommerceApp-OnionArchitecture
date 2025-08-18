using EcommerceApp.API.Endpoints;
using EcommerceApp.Core.Interfaces;
using EcommerceApp.Core.Services;
using EcommerceApp.Domain.Auth.DTOs.Request;
using EcommerceApp.Domain.Auth.Interfaces;
using EcommerceApp.Domain.Auth.Repository;
using EcommerceApp.Domain.Auth.Service;
using EcommerceApp.Domain.Auth.Validations;
using EcommerceApp.Domain.Category.DTOs.Request;
using EcommerceApp.Domain.Category.Interfaces;
using EcommerceApp.Domain.Category.Repository;
using EcommerceApp.Domain.Category.Service;
using EcommerceApp.Domain.Category.Validations;
using EcommerceApp.Domain.Product.DTOs.Request;
using EcommerceApp.Domain.Product.Interfaces;
using EcommerceApp.Domain.Product.Repository;
using EcommerceApp.Domain.Product.Service;
using EcommerceApp.Domain.Product.Validations;
using EcommerceApp.Domain.User.DTOs.Request;
using EcommerceApp.Domain.User.Interfaces;
using EcommerceApp.Domain.User.Repository;
using EcommerceApp.Domain.User.Service;
using EcommerceApp.Domain.User.Validations;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Token"]!)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidateLifetime = true,
        };
    });

// Add Authorization policies
builder.Services.AddAuthorization(options =>
{
    // Role-based policies
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Administrator"));
    options.AddPolicy("SellerOnly", policy => policy.RequireRole("Seller"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
    
    // Combined role policies
    options.AddPolicy("AdminOrSeller", policy => 
        policy.RequireRole("Administrator", "Seller"));
    options.AddPolicy("AllUsers", policy => 
        policy.RequireRole("Administrator", "Seller", "User"));
    
    // Custom claim-based policies
    options.AddPolicy("IsAdmin", policy => 
        policy.RequireClaim("isAdmin", "1"));
    options.AddPolicy("IsSeller", policy => 
        policy.RequireClaim("isSeller", "1"));
    options.AddPolicy("IsUser", policy => 
        policy.RequireClaim("isUser", "1"));
    
    // Require authentication for any endpoint
    options.AddPolicy("RequireAuth", policy => 
        policy.RequireAuthenticatedUser());
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:4200") // Specify your Angular app's origin
                           .AllowAnyHeader()
                           .AllowAnyMethod());
});

// Registering services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IValidator<AddCategory>, CreateCategoryRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateCategory>, UpdateCategoryRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateProduct>, UpdateProductRequestValidator>();
builder.Services.AddScoped<IValidator<CreateProduct>, CreateProductRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateUser>, UpdateUserRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateUserRoles>, UpdateUserRolesValidator>();
builder.Services.AddScoped<UpdateUserRequestValidator>();
builder.Services.AddScoped<IValidator<SignupRequest>, SignUpUserRequestValidator>();
builder.Services.AddScoped<IValidator<LoginRequest>, LoginUserRequestValidator>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuditService, AuditService>();


var app = builder.Build();
app.UseCors("AllowSpecificOrigin");
app.UseAuthentication();
app.UseAuthorization();

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
AuthEndpoints.MapAuthEndpoints(app);
app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
