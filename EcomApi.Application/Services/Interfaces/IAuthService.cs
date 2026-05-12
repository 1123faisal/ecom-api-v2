using System;
using EcomApi.Application.DTOs.Auth;

namespace EcomApi.Application.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> RegisterAdminAsync(RegisterDto dto);
    Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);
}
