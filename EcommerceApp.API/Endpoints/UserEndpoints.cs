using EcommerceApp.Core.DTOs;
using EcommerceApp.Domain.Category.DTOs.Request;
using EcommerceApp.Domain.Category.Interfaces;
using EcommerceApp.Domain.User.DTOs.Request;
using EcommerceApp.Domain.User.Interfaces;

namespace EcommerceApp.API.Endpoints
{
    public static class UserEndpoints
    {
        public static void MapUserEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/users")
                .WithTags("Users")
                .WithOpenApi();
            group.MapGet("/roles", async (IUserService userService) =>
            {
                var roles = await userService.GetRoles();
                return Results.Ok(roles);
            }).WithSummary("Get all roles");
            
            group.MapGet("/", async (IUserService userService) =>
            {
                var users = await userService.GetUsersAsync();
                return Results.Ok(users);
            }).WithSummary("Get all users");
            group.MapPut("/{id:int}", async (IUserService userService, int id, UpdateUser updateUserRequest) =>
            {
                var result = await userService.UpdateUser(id, updateUserRequest);
                
                if (!result)
                {
                    return Results.BadRequest("Failed to update user.");
                }
                
                return Results.Ok();
            }).WithSummary("Update user details");
            group.MapPut("/update-role", async (IUserService userService, UpdateUserRoles updateUserRoles) =>
            {
                var result = await userService.UpdateUserRole(updateUserRoles);
                
                return Results.Ok();
            }).WithSummary("Update user roles");
        }
    }
}
