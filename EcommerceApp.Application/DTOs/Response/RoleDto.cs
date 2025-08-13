using System.ComponentModel.DataAnnotations;

namespace EcommerceApp.Application.DTOs.Response
{
    public class RoleDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

    }
}
