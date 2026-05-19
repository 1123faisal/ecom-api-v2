using System.ComponentModel.DataAnnotations;

namespace EcomApi.API.Options;

public sealed class AdminOptions
{
    public const string SectionName = "Admin";

    [Required]
    public required string Secret { get; init; }
}
