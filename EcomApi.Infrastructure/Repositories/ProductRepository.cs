using System;
using EcomApi.Domain.Entities;
using EcomApi.Domain.Interfaces;
using EcomApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EcomApi.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private AppDbContext _db;

    public ProductRepository(AppDbContext context)
    {
        _db = context;
    }

    public async Task<Product> CreateAsync(Product product)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return await GetByIdAsync(product.Id) ?? product;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null)
            return false;

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await _db.Products.ToListAsync();
    }

    public async Task<List<Product>> GetByCategoryAsync(int categoryId)
    {
        return await _db
            .Products.Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _db.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product?> UpdateAsync(int id, Product updated)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null)
            return null;

        product.Name = updated.Name;
        product.Description = updated.Description;
        product.Price = updated.Price;
        product.Stock = updated.Stock;
        product.CategoryId = updated.CategoryId;
        await _db.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task<bool> UpdateStockAsync(int id, int quantity)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null)
            return false;

        product.Stock -= quantity;
        await _db.SaveChangesAsync();
        return true;
    }
}
