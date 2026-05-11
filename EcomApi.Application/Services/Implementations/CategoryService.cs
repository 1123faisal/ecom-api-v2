using System;
using EcomApi.Application.DTOs.Category;
using EcomApi.Application.Services.Interfaces;
using EcomApi.Domain.Entities;
using EcomApi.Domain.Interfaces;

namespace EcomApi.Application.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto)
    {
        var category = new Category { Name = dto.Name, Description = dto.Description };
        var created = await _categoryRepository.CreateAsync(category);
        return MapToDto(created);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _categoryRepository.DeleteAsync(id);
    }

    public async Task<List<CategoryResponseDto>> GetAllAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return categories.Select(MapToDto).ToList();
    }

    public async Task<CategoryResponseDto?> GetByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        return category == null ? null : MapToDto(category);
    }

    public async Task<CategoryResponseDto?> UpdateAsync(int id, UpdateCategoryDto dto)
    {
        var updated = new Category { Name = dto.Name, Description = dto.Description };
        var category = await _categoryRepository.UpdateAsync(id, updated);
        return category == null ? null : MapToDto(category);
    }

    private CategoryResponseDto MapToDto(Category c) =>
        new(c.Id, c.Name, c.Description, c.Products?.Count ?? 0);
}
