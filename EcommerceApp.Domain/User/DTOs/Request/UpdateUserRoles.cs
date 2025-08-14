using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Domain.User.DTOs.Request
{
    public class UpdateUserRoles
    {
        public int Id { get; set; }
        public int[] RoleIds { get; set; }
    }
}
