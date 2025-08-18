using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Core.Interfaces
{
    public interface IAuditService
    {
        int? GetCurrentUserId();
        DateTime GetCurrentDateTime();
    }
}
