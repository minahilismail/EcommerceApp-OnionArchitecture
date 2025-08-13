using System.ComponentModel.DataAnnotations;
using System.Data;

namespace EcommerceApp.Application.DTOs.Response
{
    public class UserDto
    {
        public int Id { get; set; }

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

        [Required]
        public string Password { get; set; }
        public bool IsActive { get; set; } = true;
        public string[]? Roles { get; set; }

    }
}
