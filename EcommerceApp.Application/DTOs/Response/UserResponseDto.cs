namespace EcommerceApp.Application.DTOs.Response
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public DateTime CreatedOn { get; set; }
    }
}
