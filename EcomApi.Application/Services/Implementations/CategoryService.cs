using EcomApi.Application.DTOs.Category;
using EcomApi.Application.Services.Interfaces;
using EcomApi.Domain.Entities;
using EcomApi.Domain.Exceptions;
using EcomApi.Domain.Interfaces;

namespace EcomApi.Application.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto, CancellationToken ct = default)
    {
        var category = new Category { Name = dto.Name, Description = dto.Description };
        var created = await _categoryRepository.CreateAsync(category, ct);
        return MapToDto(created);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var exists = await _categoryRepository.GetByIdAsync(id, ct);
        if (exists == null)
            throw new NotFoundException($"Category {id} not found.");

        return await _categoryRepository.DeleteAsync(id, ct);
    }

    public async Task<List<CategoryResponseDto>> GetAllAsync(CancellationToken ct = default)
    {
        var categories = await _categoryRepository.GetAllAsync(ct);
        return categories.Select(MapToDto).ToList();
    }

    public async Task<CategoryResponseDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, ct);
        return category == null ? null : MapToDto(category);
    }

    public async Task<CategoryResponseDto?> UpdateAsync(int id, UpdateCategoryDto dto, CancellationToken ct = default)
    {
        var updated = new Category { Name = dto.Name, Description = dto.Description };
        var category = await _categoryRepository.UpdateAsync(id, updated, ct);
        return category == null ? null : MapToDto(category);
    }

    private static CategoryResponseDto MapToDto(Category c) =>
        new(c.Id, c.Name, c.Description, c.Products?.Count ?? 0);
}

