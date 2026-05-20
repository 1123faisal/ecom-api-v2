using System.ComponentModel.DataAnnotations;

namespace EcomApi.Application.DTOs.Category;

public record CreateCategoryDto(
    [Required, MinLength(2), MaxLength(100)] string Name,
    [MaxLength(500)] string? Description
);

public record UpdateCategoryDto(
    [Required, MinLength(2), MaxLength(100)] string Name,
    [MaxLength(500)] string? Description
);

public record CategoryResponseDto(int Id, string Name, string? Description, int ProductCount);
