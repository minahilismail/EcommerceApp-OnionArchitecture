using System.ComponentModel.DataAnnotations;

namespace EcommerceApp.Application.DTOs.Request
{
    public class UpdateUserDto
    {

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        public bool IsActive { get; set; } = true;

        public int[]? RoleIds { get; set; }
    }
}
