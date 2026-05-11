namespace EcomApi.Application.DTOs.Category;

public record CreateCategoryDto(string Name, string Description);

public record UpdateCategoryDto(string Name, string Description);

public record CategoryResponseDto(int Id, string Name, string Description, int ProductCount);
