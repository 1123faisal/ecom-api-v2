using System.ComponentModel.DataAnnotations;

namespace EcomApi.Application.DTOs.Category;

public record CreateCategoryDto(
    [property: Required, MinLength(2), MaxLength(100)] string Name,
    [property: MaxLength(500)] string? Description
);

public record UpdateCategoryDto(
    [property: Required, MinLength(2), MaxLength(100)] string Name,
    [property: MaxLength(500)] string? Description
);

public record CategoryResponseDto(int Id, string Name, string? Description, int ProductCount);

