using EcommerceApp.Application.DTOs.Request;
using EcommerceApp.Application.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<UserResponseDto?> RegisterAsync(RegisterRequestDto request);
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
    }
}
