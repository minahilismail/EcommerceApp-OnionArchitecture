using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Domain.Product.DTOs.Request
{
    public class ProductFilter
    {
        public int? CategoryId { get; set; }
        public string? SearchTerm { get; set; }
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }

    }
}
