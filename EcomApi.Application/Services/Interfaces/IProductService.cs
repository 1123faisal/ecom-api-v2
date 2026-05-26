using EcomApi.Application.DTOs.Product;

namespace EcomApi.Application.Services.Interfaces;

public interface IProductService
{
    Task<List<ProductResponseDto>> GetAllAsync(CancellationToken ct = default);
    Task<ProductResponseDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<ProductResponseDto>> GetByCategoryAsync(int categoryId, CancellationToken ct = default);
    Task<ProductResponseDto> CreateAsync(CreateProductDto dto, CancellationToken ct = default);
    Task<ProductResponseDto?> UpdateAsync(int id, UpdateProductDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
