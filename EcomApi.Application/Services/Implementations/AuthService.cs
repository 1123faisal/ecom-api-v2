using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EcomApi.Application.DTOs.Auth;
using EcomApi.Application.Options;
using EcomApi.Application.Services.Interfaces;
using EcomApi.Domain.Entities;
using EcomApi.Domain.Enums;
using EcomApi.Domain.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EcomApi.Application.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtOptions _jwt;

    public AuthService(IUserRepository userRepository, IOptions<JwtOptions> jwtOptions)
    {
        _userRepository = userRepository;
        _jwt = jwtOptions.Value;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByUsernameAsync(dto.Username, ct);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return null;

        return ToResponse(user);
    }

    public async Task<AuthResponseDto?> RegisterAdminAsync(RegisterDto dto, CancellationToken ct = default)
    {
        if (await _userRepository.ExistsAsync(dto.Username, dto.Email, ct))
            return null;

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = UserRole.Admin,
        };

        var created = await _userRepository.AddAsync(user, ct);
        return ToResponse(created);
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto, CancellationToken ct = default)
    {
        if (await _userRepository.ExistsAsync(dto.Username, dto.Email, ct))
            return null;

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = UserRole.Customer,
        };

        var created = await _userRepository.AddAsync(user, ct);
        return ToResponse(created);
    }

    private AuthResponseDto ToResponse(User user) =>
        new(GenerateToken(user), user.Username, user.Email, user.Role.ToString());

    private string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_jwt.ExpiryHours),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

