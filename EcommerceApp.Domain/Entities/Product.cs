using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceApp.Domain.Entities
{
    public class Product : AuditableEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }
        public double Price { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; }

        [MaxLength(500)]
        public string? Image { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

    }
}
