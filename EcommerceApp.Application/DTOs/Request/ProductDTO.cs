using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace EcommerceApp.Application.DTOs.Request
{
    public class ProductDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public double Price { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; }

        public string? Image { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public string? CategoryName { get; set; }
    }

    public class CreateProductDto
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

    public class UpdateProductDto
    {
        public int Id { get; set; }

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
