using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Domain.Product.DTOs.Request
{
    public class CreateProduct
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public double Price { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; }

        public IFormFile? Image { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
