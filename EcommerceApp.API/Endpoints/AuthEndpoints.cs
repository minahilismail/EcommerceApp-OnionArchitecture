using EcommerceApp.Domain.Auth.DTOs.Request;
using EcommerceApp.Domain.Auth.Interfaces;

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
                if (result == null)
                {
                    return Results.BadRequest("Invalid email or password.");
                }
                return Results.Ok(result);
            }).WithSummary("Login user");
            
            group.MapPost("/signup", async (IAuthService authService, SignupRequest signupRequest) =>
            {
                var result = await authService.SignUpAsync(signupRequest);
                if (result == null)
                {
                    return Results.BadRequest("User already exists or registration failed.");
                }
                return Results.Ok(result);
            }).WithSummary("Sign up user");
        }
    }
}
