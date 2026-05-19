using EcomApi.Application.DTOs.Category;

namespace EcomApi.Application.Services.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryResponseDto>> GetAllAsync(CancellationToken ct = default);
    Task<CategoryResponseDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto, CancellationToken ct = default);
    Task<CategoryResponseDto?> UpdateAsync(int id, UpdateCategoryDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
