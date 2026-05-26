using System.ComponentModel.DataAnnotations;

namespace EcomApi.Application.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    /// <summary>HMAC-SHA256 signing key — must be at least 32 characters (256 bits).</summary>
    [Required, MinLength(32)]
    public required string Key { get; init; }

    [Required]
    public required string Issuer { get; init; }

    [Required]
    public required string Audience { get; init; }

    [Range(0.1, 720)]
    public double ExpiryHours { get; init; } = 24;
}
