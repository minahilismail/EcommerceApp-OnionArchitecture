using EcommerceApp.Domain.Auth.DTOs.Request;
using EcommerceApp.Domain.Auth.Interfaces;
using EcommerceApp.Domain.Category.Interfaces;

namespace EcommerceApp.API.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/auth")
                .WithTags("Authentication")
                .WithOpenApi();
            group.MapPost("/login", async (IAuthService authService, LoginRequest loginRequest) =>
            {
                var result = await authService.LoginAsync(loginRequest);
                return Results.Ok(result);
            }).WithSummary("Login user");
            group.MapPost("/signup", async (IAuthService authService, SignupRequest signupRequest) =>
            {
                var result = await authService.SignUpAsync(signupRequest);
                return Results.Ok(result);
            }).WithSummary("Sign up user");
        }
        }
}
