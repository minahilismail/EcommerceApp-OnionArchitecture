using EcommerceApp.Domain.Auth.DTOs.Request;
using EcommerceApp.Domain.Auth.DTOs.Response;
using EcommerceApp.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Domain.Auth.Mappings
{
    public static class AuthExtensions
    {
        // SignupRequest to UserModel mapping
        public static UserModel ToEntity(this SignupRequest request)
        {
            return new UserModel
            {
                Name = request.Name,
                Username = request.Username,
                Email = request.Email,
                Password = request.Password, // Will be hashed in service
                IsActive = true,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow
            };
        }

        // UserModel to SignupResponse mapping
        public static SignupResponse ToSignupResponse(this UserModel user, string[] roles)
        {
            return new SignupResponse
            {
                Id = user.Id,
                Name = user.Name,
                Username = user.Username,
                Email = user.Email,
                IsActive = user.IsActive,
                Roles = roles
            };
        }

        // UserModel to LoginResponse mapping
        public static LoginResponse ToLoginResponse(this UserModel user, string token)
        {
            return new LoginResponse
            {
                Token = token
            };
        }
    }
}
