using System.ComponentModel.DataAnnotations;

namespace EcomApi.Application.DTOs.Auth;

public record RegisterDto(
    [Required, MinLength(3), MaxLength(50)] string Username,
    [Required, EmailAddress, MaxLength(256)] string Email,
    [Required, MinLength(6), MaxLength(100)] string Password
);

public record LoginDto([Required] string Username, [Required] string Password);

public record AuthResponseDto(string Token, string Username, string Email, string Role);
