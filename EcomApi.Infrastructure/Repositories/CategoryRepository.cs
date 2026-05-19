using EcomApi.Domain.Entities;
using EcomApi.Domain.Interfaces;
using EcomApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EcomApi.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _db;

    public CategoryRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Category> CreateAsync(Category category, CancellationToken ct = default)
    {
        _db.Categories.Add(category);
        await _db.SaveChangesAsync(ct);
        return category;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var category = await _db.Categories.FindAsync([id], ct);
        if (category == null)
            return false;

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<List<Category>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.Categories
            .AsNoTracking()
            .Include(c => c.Products)
            .ToListAsync(ct);
    }

    public async Task<Category?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _db.Categories
            .AsNoTracking()
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<Category?> UpdateAsync(int id, Category updated, CancellationToken ct = default)
    {
        var category = await _db.Categories.FindAsync([id], ct);
        if (category == null)
            return null;

        category.Name = updated.Name;
        category.Description = updated.Description;
        await _db.SaveChangesAsync(ct);

        return await _db.Categories
            .AsNoTracking()
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }
}

