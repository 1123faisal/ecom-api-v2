using EcomApi.Application.DTOs.Auth;

namespace EcomApi.Application.Services.Interfaces;

public interface IAuthService
{
    /// <summary>Throws <see cref="EcomApi.Domain.Exceptions.ConflictException"/> if username/email already exists.</summary>
    Task<AuthResponseDto> RegisterAdminAsync(RegisterDto dto, CancellationToken ct = default);

    /// <summary>Throws <see cref="EcomApi.Domain.Exceptions.ConflictException"/> if username/email already exists.</summary>
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto, CancellationToken ct = default);

    /// <summary>Returns null when credentials are invalid (intentional — do not differentiate).</summary>
    Task<AuthResponseDto?> LoginAsync(LoginDto dto, CancellationToken ct = default);
}
