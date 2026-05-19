namespace EcomApi.Application.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public required string Key { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public double ExpiryHours { get; init; } = 24;
}
