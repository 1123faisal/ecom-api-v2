using EcomApi.Domain.Entities;
using EcomApi.Domain.Interfaces;
using EcomApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EcomApi.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _db;

    public ProductRepository(AppDbContext context)
    {
        _db = context;
    }

    public async Task<Product> CreateAsync(Product product, CancellationToken ct = default)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync(ct);
        return await GetByIdAsync(product.Id, ct) ?? product;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var product = await _db.Products.FindAsync([id], ct);
        if (product == null)
            return false;

        _db.Products.Remove(product);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<List<Product>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .ToListAsync(ct);
    }

    public async Task<List<Product>> GetByCategoryAsync(int categoryId, CancellationToken ct = default)
    {
        return await _db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync(ct);
    }

    public async Task<Product?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<Product?> UpdateAsync(int id, Product updated, CancellationToken ct = default)
    {
        var product = await _db.Products.FindAsync([id], ct);
        if (product == null)
            return null;

        product.Name = updated.Name;
        product.Description = updated.Description;
        product.Price = updated.Price;
        product.Stock = updated.Stock;
        product.CategoryId = updated.CategoryId;
        await _db.SaveChangesAsync(ct);
        return await GetByIdAsync(id, ct);
    }

    public async Task<bool> DecrementStockAsync(int id, int quantity, CancellationToken ct = default)
    {
        var product = await _db.Products.FindAsync([id], ct);
        if (product == null)
            return false;

        product.Stock -= quantity;
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
