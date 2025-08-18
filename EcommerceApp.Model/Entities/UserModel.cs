using System.ComponentModel.DataAnnotations;
using System.Data;

namespace EcommerceApp.Model.Entities
{
    public class UserModel : AuditableEntity
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

        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        //public string? RefreshToken { get; set; }
        //public DateTime RefreshTokenExpiryTime { get; set; }

    }
}
