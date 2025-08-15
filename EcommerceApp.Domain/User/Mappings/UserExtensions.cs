using EcommerceApp.Domain.Product.DTOs.Request;
using EcommerceApp.Domain.Product.DTOs.Response;
using EcommerceApp.Domain.User.DTOs.Request;
using EcommerceApp.Domain.User.DTOs.Response;
using EcommerceApp.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Domain.User.Mappings
{
    public static class UserExtensions
    {
        // Domain to DTO mappings
        public static UserResponse ToDto(this UserModel user)
        {
            return new UserResponse
            {
               Id = user.Id,
               Name = user.Name,
               Username = user.Username,
               Email = user.Email,
               IsActive = user.IsActive,
               Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()


            };
        }
        public static UserRoleDto ToRoleDto(this RoleModel roles)
        {
            return new UserRoleDto
            {
                Id = roles.Id,
                Name = roles.Name,
            };
        }

        public static IEnumerable<UserRoleDto> ToRoleDto(this IEnumerable<RoleModel> roles)
        {
            return roles.Select(r => r.ToRoleDto());
        }

        public static IEnumerable<UserResponse> ToDto(this IEnumerable<UserModel> users)
        {
            return users.Select(u => u.ToDto());
        }

    
        public static void UpdateFromDto(this UserModel entity, UpdateUser dto)
        {
            entity.Name = dto.Name;
            entity.Username = dto.Username;
            entity.IsActive = dto.IsActive;
        }

    }
}
