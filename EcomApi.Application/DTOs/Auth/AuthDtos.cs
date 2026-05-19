using System.ComponentModel.DataAnnotations;

namespace EcomApi.Application.DTOs.Auth;

public record RegisterDto(
    [property: Required, MinLength(3), MaxLength(50)] string Username,
    [property: Required, EmailAddress, MaxLength(256)] string Email,
    [property: Required, MinLength(6), MaxLength(100)] string Password
);

public record LoginDto(
    [property: Required] string Username,
    [property: Required] string Password
);

public record AuthResponseDto(string Token, string Username, string Email, string Role);

