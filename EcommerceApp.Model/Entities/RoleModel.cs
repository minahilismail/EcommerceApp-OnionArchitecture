using System.ComponentModel.DataAnnotations;

namespace EcommerceApp.Model.Entities
{
    public class RoleModel : AuditableEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    }
}
