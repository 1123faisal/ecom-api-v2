using System;
using EcomApi.Application.DTOs.Product;
using EcomApi.Application.Services.Interfaces;
using EcomApi.Domain.Entities;
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

    public async Task<ProductResponseDto> CreateAsync(CreateProductDto dto)
    {
        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
        if (category == null)
            throw new Exception($"Category {dto.CategoryId} not found.");

        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            CategoryId = dto.CategoryId,
        };

        var created = await _productRepository.CreateAsync(product);
        return MapToDto(created);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _productRepository.DeleteAsync(id);
    }

    public async Task<List<ProductResponseDto>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(MapToDto).ToList();
    }

    public async Task<List<ProductResponseDto>> GetByCategoryAsync(int categoryId)
    {
        var products = await _productRepository.GetByCategoryAsync(categoryId);
        return products.Select(MapToDto).ToList();
    }

    public async Task<ProductResponseDto?> GetByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return product == null ? null : MapToDto(product);
    }

    public async Task<ProductResponseDto?> UpdateAsync(int id, UpdateProductDto dto)
    {
        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
        if (category == null)
            throw new Exception($"Category {dto.CategoryId} not found.");

        var updated = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            CategoryId = dto.CategoryId,
        };

        var product = await _productRepository.UpdateAsync(id, updated);
        return product == null ? null : MapToDto(product);
    }

    private ProductResponseDto MapToDto(Product p) =>
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
