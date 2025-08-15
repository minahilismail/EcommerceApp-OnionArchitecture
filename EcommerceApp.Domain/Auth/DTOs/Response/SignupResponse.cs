using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Domain.Auth.DTOs.Response
{
    public class SignupResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }   
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; } = true;
        public string[]? Roles { get; set; }

    }
}
