using EcommerceApp.Core.DTOs;
using EcommerceApp.Domain.Category.DTOs.Request;
using EcommerceApp.Domain.Category.Interfaces;
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
            

        }
    }
}
