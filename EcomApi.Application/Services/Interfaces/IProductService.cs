using System;
using EcomApi.Application.DTOs.Product;

namespace EcomApi.Application.Services.Interfaces;

public interface IProductService
{
    Task<List<ProductResponseDto>> GetAllAsync();
    Task<ProductResponseDto?> GetByIdAsync(int id);
    Task<List<ProductResponseDto>> GetByCategoryAsync(int categoryId);
    Task<ProductResponseDto> CreateAsync(CreateProductDto dto);
    Task<ProductResponseDto?> UpdateAsync(int id, UpdateProductDto dto);
    Task<bool> DeleteAsync(int id);
}
