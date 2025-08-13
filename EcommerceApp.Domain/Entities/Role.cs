using System.ComponentModel.DataAnnotations;

namespace EcommerceApp.Domain.Entities
{
    public class Role : AuditableEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    }
}
