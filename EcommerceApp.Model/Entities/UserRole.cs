namespace EcommerceApp.Model.Entities
{
    public class UserRole
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }

        
        public UserModel User { get; set; }
        public RoleModel Role { get; set; }
    }
}
