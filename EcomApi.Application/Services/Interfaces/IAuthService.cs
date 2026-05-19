using EcomApi.Application.DTOs.Auth;

namespace EcomApi.Application.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> RegisterAdminAsync(RegisterDto dto, CancellationToken ct = default);
    Task<AuthResponseDto?> RegisterAsync(RegisterDto dto, CancellationToken ct = default);
    Task<AuthResponseDto?> LoginAsync(LoginDto dto, CancellationToken ct = default);
}
