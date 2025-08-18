using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceApp.Model.Entities
{
    public class CategoryModel : AuditableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        // Parent-Child relationship properties
        public int? ParentCategoryId { get; set; }
        public int? StatusId { get; set; } = 1;

        [ForeignKey("ParentCategoryId")]
        public CategoryModel? ParentCategory { get; set; }
        [ForeignKey("StatusId")]
        public StatusModel Status { get; set; }

        public ICollection<CategoryModel> SubCategories { get; set; } = new List<CategoryModel>();
    }
}
