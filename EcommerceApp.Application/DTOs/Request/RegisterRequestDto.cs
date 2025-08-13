using System.ComponentModel.DataAnnotations;

namespace EcommerceApp.Application.DTOs.Request
{
    public class RegisterRequestDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

    }
}
