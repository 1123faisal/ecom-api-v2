using EcomApi.Application.Abstractions;
using EcomApi.Application.DTOs.Category;
using EcomApi.Application.Services.Interfaces;
using EcomApi.Domain.Entities;
using EcomApi.Domain.Exceptions;
using EcomApi.Domain.Interfaces;

namespace EcomApi.Application.Services.Implementations;

public class CategoryService : ICategoryService
{
    private const string CachePrefix = "categories:";
    private const string AllCategoriesKey = "categories:all";

    private readonly ICategoryRepository _categoryRepository;
    private readonly ICacheService _cache;

    public CategoryService(ICategoryRepository categoryRepository, ICacheService cache)
    {
        _categoryRepository = categoryRepository;
        _cache = cache;
    }

    public async Task<CategoryResponseDto> CreateAsync(
        CreateCategoryDto dto,
        CancellationToken ct = default
    )
    {
        var category = new Category { Name = dto.Name, Description = dto.Description };
        var created = await _categoryRepository.CreateAsync(category, ct);
        await _cache.RemoveAsync(AllCategoriesKey, ct);
        return MapToDto(created);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var exists = await _categoryRepository.GetByIdAsync(id, ct);
        if (exists == null)
            throw new NotFoundException($"Category {id} not found.");

        var result = await _categoryRepository.DeleteAsync(id, ct);
        await _cache.RemoveAsync(AllCategoriesKey, ct);
        await _cache.RemoveAsync($"{CachePrefix}{id}", ct);
        return result;
    }

    public async Task<List<CategoryResponseDto>> GetAllAsync(CancellationToken ct = default)
    {
        var cached = await _cache.GetAsync<List<CategoryResponseDto>>(AllCategoriesKey, ct);
        if (cached != null)
            return cached;

        var categories = await _categoryRepository.GetAllAsync(ct);
        var result = categories.Select(MapToDto).ToList();
        await _cache.SetAsync(AllCategoriesKey, result, TimeSpan.FromMinutes(10), ct);
        return result;
    }

    public async Task<CategoryResponseDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var key = $"{CachePrefix}{id}";
        var cached = await _cache.GetAsync<CategoryResponseDto>(key, ct);
        if (cached != null)
            return cached;

        var category = await _categoryRepository.GetByIdAsync(id, ct);
        if (category == null)
            return null;

        var result = MapToDto(category);
        await _cache.SetAsync(key, result, TimeSpan.FromMinutes(10), ct);
        return result;
    }

    public async Task<CategoryResponseDto?> UpdateAsync(
        int id,
        UpdateCategoryDto dto,
        CancellationToken ct = default
    )
    {
        var updated = new Category { Name = dto.Name, Description = dto.Description };
        var category = await _categoryRepository.UpdateAsync(id, updated, ct);
        if (category == null)
            return null;

        await _cache.RemoveAsync(AllCategoriesKey, ct);
        await _cache.RemoveAsync($"{CachePrefix}{id}", ct);
        return MapToDto(category);
    }

    private static CategoryResponseDto MapToDto(Category c) =>
        new(c.Id, c.Name, c.Description, c.Products?.Count ?? 0);
}
