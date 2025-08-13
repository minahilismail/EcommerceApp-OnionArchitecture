using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Application.DTOs.Response
{
    public class CategoryResponse
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
        [Required]
        public string Code { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        public int? ParentCategoryId { get; set; }
        public int? StatusId { get; set; }
        public string? ParentCategoryName { get; set; }
        public List<CategoryDTO>? SubCategories { get; set; }
    }
}
