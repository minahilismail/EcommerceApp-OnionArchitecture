using EcommerceApp.Core.Interfaces;
using EcommerceApp.Model.Entities;
using Microsoft.Extensions.Configuration;

namespace EcommerceApp.Core.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly string _connectionString;
        protected readonly IAuditService _auditService;

        protected BaseRepository(IConfiguration configuration, IAuditService auditService)
        {
            _connectionString = configuration.GetConnectionString("DefaultSQLConnection")!;
            _auditService = auditService;
        }

        protected void SetAuditFieldsForCreate<T>(T entity) where T : AuditableEntity
        {
            var now = _auditService.GetCurrentDateTime();
            var userId = _auditService.GetCurrentUserId();

            entity.CreatedOn = now;
            entity.CreatedBy = userId;
            entity.UpdatedOn = null;
            entity.UpdatedBy = null;
        }

        protected void SetAuditFieldsForUpdate<T>(T entity) where T : AuditableEntity
        {
            var now = _auditService.GetCurrentDateTime();
            var userId = _auditService.GetCurrentUserId();

            entity.UpdatedOn = now;
            entity.UpdatedBy = userId;
        }
    }
}