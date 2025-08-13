using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceApp.Domain.Entities
{
    public class Category : AuditableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        // Parent-Child relationship properties
        public int? ParentCategoryId { get; set; }
        public int? StatusId { get; set; } = 1;

        [ForeignKey("ParentCategoryId")]
        public Category? ParentCategory { get; set; }
        [ForeignKey("StatusId")]
        public Status Status { get; set; }

        public ICollection<Category> SubCategories { get; set; } = new List<Category>();
    }
}
