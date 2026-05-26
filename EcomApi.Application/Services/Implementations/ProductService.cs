using EcomApi.Application.DTOs.Product;
using EcomApi.Application.Services.Interfaces;
using EcomApi.Domain.Entities;
using EcomApi.Domain.Exceptions;
using EcomApi.Domain.Interfaces;

namespace EcomApi.Application.Services.Implementations;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository
    )
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<ProductResponseDto> CreateAsync(CreateProductDto dto, CancellationToken ct = default)
    {
        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId, ct);
        if (category == null)
            throw new NotFoundException($"Category {dto.CategoryId} not found.");

        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            CategoryId = dto.CategoryId,
        };

        var created = await _productRepository.CreateAsync(product, ct);
        return MapToDto(created);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var exists = await _productRepository.GetByIdAsync(id, ct);
        if (exists == null)
            throw new NotFoundException($"Product {id} not found.");

        return await _productRepository.DeleteAsync(id, ct);
    }

    public async Task<List<ProductResponseDto>> GetAllAsync(CancellationToken ct = default)
    {
        var products = await _productRepository.GetAllAsync(ct);
        return products.Select(MapToDto).ToList();
    }

    public async Task<List<ProductResponseDto>> GetByCategoryAsync(int categoryId, CancellationToken ct = default)
    {
        var products = await _productRepository.GetByCategoryAsync(categoryId, ct);
        return products.Select(MapToDto).ToList();
    }

    public async Task<ProductResponseDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var product = await _productRepository.GetByIdAsync(id, ct);
        return product == null ? null : MapToDto(product);
    }

    public async Task<ProductResponseDto?> UpdateAsync(int id, UpdateProductDto dto, CancellationToken ct = default)
    {
        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId, ct);
        if (category == null)
            throw new NotFoundException($"Category {dto.CategoryId} not found.");

        var updated = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            CategoryId = dto.CategoryId,
        };

        var product = await _productRepository.UpdateAsync(id, updated, ct);
        return product == null ? null : MapToDto(product);
    }

    private static ProductResponseDto MapToDto(Product p) =>
        new(
            p.Id,
            p.Name,
            p.Description,
            p.Price,
            p.Stock,
            p.CategoryId,
            p.Category?.Name ?? "Unknown"
        );
}
