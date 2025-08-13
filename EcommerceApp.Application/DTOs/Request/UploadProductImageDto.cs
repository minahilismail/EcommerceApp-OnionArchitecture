using System.ComponentModel.DataAnnotations;

namespace EcommerceApp.Application.DTOs.Request
{
    public class UploadProductImageDto
    {
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        public IFormFile Image { get; set; }
    }
}
