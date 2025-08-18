using EcommerceApp.Domain.Auth.DTOs.Request;
using EcommerceApp.Domain.Auth.Interfaces;
using System.IdentityModel.Tokens.Jwt;

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

            // Option 1: Extract from expired JWT token (Most sophisticated)
            group.MapPost("/refresh-token", async (IAuthService authService, RefreshTokenRequest request) =>
            {
                try
                {
                    // Extract user ID and refresh token from the expired access token
                    var handler = new JwtSecurityTokenHandler();
                    var jsonToken = handler.ReadJwtToken(request.ExpiredToken);
                    
                    var userIdClaim = jsonToken.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
                    var refreshTokenClaim = jsonToken.Claims.FirstOrDefault(x => x.Type == "refreshToken")?.Value;
                    
                    if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(refreshTokenClaim))
                    {
                        return Results.BadRequest("Invalid token structure.");
                    }

                    var userId = int.Parse(userIdClaim);
                    var result = await authService.RefreshTokenAsync(userId, refreshTokenClaim);
                    
                    if(result == null)
                    {
                        return Results.BadRequest("Invalid refresh token.");
                    }
                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest($"Token refresh failed: {ex.Message}");
                }
            }).WithSummary("Refresh user token using expired JWT");

            // Option 2: Direct refresh token approach (Simpler)
            group.MapPost("/refresh-token-direct", async (IAuthService authService, DirectRefreshRequest request) =>
            {
                try
                {
                    var result = await authService.RefreshTokenAsync(request.UserId, request.RefreshToken);
                    
                    if(result == null)
                    {
                        return Results.BadRequest("Invalid refresh token.");
                    }
                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest($"Token refresh failed: {ex.Message}");
                }
            }).WithSummary("Refresh user token directly");
        }
    }

    // Request models for different refresh approaches
    public class RefreshTokenRequest
    {
        public string ExpiredToken { get; set; } = string.Empty;
    }

    public class DirectRefreshRequest
    {
        public int UserId { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
    }
}
