using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Model.Entities.Category
{
    public class CreateUpdateCategoryModel : AuditableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        public int? ParentCategoryId { get; set; }
        public int? StatusId { get; set; } = 1;
    }
}
