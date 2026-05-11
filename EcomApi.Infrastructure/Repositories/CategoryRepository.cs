using System;
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

    public async Task<Category> CreateAsync(Category category)
    {
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();
        return category;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category == null)
            return false;
        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<Category>> GetAllAsync()
    {
        return await _db.Categories.Include(c => c.Products).ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _db.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Category?> UpdateAsync(int id, Category updated)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category == null)
            return null;

        category.Name = updated.Name;
        category.Description = updated.Description;
        await _db.SaveChangesAsync();
        return await GetByIdAsync(id) ?? category;
    }
}
