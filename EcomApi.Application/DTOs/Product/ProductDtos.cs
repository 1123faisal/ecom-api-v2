using System.ComponentModel.DataAnnotations;

namespace EcomApi.Application.DTOs.Product;

public record CreateProductDto(
    [Required, MinLength(2), MaxLength(200)] string Name,
    [MaxLength(2000)] string? Description,
    [Required, Range(0.01, 10_000_000, ErrorMessage = "Price must be between 0.01 and 10,000,000.")] decimal Price,
    [Required, Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative.")] int Stock,
    [Required, Range(1, int.MaxValue, ErrorMessage = "CategoryId must be a positive integer.")] int CategoryId
);

public record UpdateProductDto(
    [Required, MinLength(2), MaxLength(200)] string Name,
    [MaxLength(2000)] string? Description,
    [Required, Range(0.01, 10_000_000, ErrorMessage = "Price must be between 0.01 and 10,000,000.")] decimal Price,
    [Required, Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative.")] int Stock,
    [Required, Range(1, int.MaxValue, ErrorMessage = "CategoryId must be a positive integer.")] int CategoryId
);

public record ProductResponseDto(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    int Stock,
    int CategoryId,
    string CategoryName
);
