using System;
using EcomApi.Domain.Entities;

namespace EcomApi.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
    Task<Category> CreateAsync(Category category);
    Task<Category?> UpdateAsync(int id, Category updated);
    Task<bool> DeleteAsync(int id);
}
