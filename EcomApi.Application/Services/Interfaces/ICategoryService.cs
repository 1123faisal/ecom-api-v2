using System;
using EcomApi.Application.DTOs.Category;

namespace EcomApi.Application.Services.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryResponseDto>> GetAllAsync();
    Task<CategoryResponseDto?> GetByIdAsync(int id);
    Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto);
    Task<CategoryResponseDto?> UpdateAsync(int id, UpdateCategoryDto dto);
    Task<bool> DeleteAsync(int id);
}
