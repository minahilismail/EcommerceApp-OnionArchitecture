using EcommerceApp.Domain.Auth.DTOs.Request;
using EcommerceApp.Domain.Auth.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Domain.Auth.Interfaces
{
    public interface IAuthRepository
    {
        public Task<LoginResponse> LoginAsync(LoginRequest loginRequest);
        public Task<SignupResponse> SignUpAsync(SignupRequest signupRequest);
    }
}
