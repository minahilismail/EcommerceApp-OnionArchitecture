using EcommerceApp.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace EcommerceApp.Core.Services
{
    public class AuditService : IAuditService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor?.HttpContext?.User?.FindFirst("userId")?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        public DateTime GetCurrentDateTime()
        {
            return DateTime.UtcNow;
        }
    }
}