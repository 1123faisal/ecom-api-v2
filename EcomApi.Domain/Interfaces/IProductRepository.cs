using System;
using EcomApi.Domain.Entities;

namespace EcomApi.Domain.Interfaces;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<List<Product>> GetByCategoryAsync(int categoryId);
    Task<Product> CreateAsync(Product product);
    Task<Product?> UpdateAsync(int id, Product updated);
    Task<bool> DeleteAsync(int id);
    Task<bool> UpdateStockAsync(int id, int quantity);
}
