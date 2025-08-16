namespace EcommerceApp.Domain.Auth.Constants
{
    public static class RoleConstants
    {
        public const string Admin = "Administrator";
        public const string Seller = "Seller"; 
        public const string User = "User";
        
        // Role IDs
        public const int AdminId = 3;
        public const int SellerId = 2;
        public const int UserId = 1;
        
        // Policy names
        public const string AdminOnly = "AdminOnly";
        public const string SellerOnly = "SellerOnly";
        public const string UserOnly = "UserOnly";
        public const string AdminOrSeller = "AdminOrSeller";
        public const string AllUsers = "AllUsers";
    }
}