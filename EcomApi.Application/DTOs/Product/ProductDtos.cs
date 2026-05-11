namespace EcomApi.Application.DTOs.Product;

public record CreateProductDto(
    string Name,
    string Description,
    double Price,
    int Stock,
    int CategoryId
);

public record UpdateProductDto(
    string Name,
    string Description,
    double Price,
    int Stock,
    int CategoryId
);

public record ProductResponseDto(
    int Id,
    string Name,
    string Description,
    double Price,
    int Stock,
    int CategoryId,
    string CategoryName
);
