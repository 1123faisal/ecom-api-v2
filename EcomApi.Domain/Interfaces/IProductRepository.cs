using EcomApi.Domain.Entities;

namespace EcomApi.Domain.Interfaces;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync(CancellationToken ct = default);
    Task<Product?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<Product>> GetByCategoryAsync(int categoryId, CancellationToken ct = default);
    Task<Product> CreateAsync(Product product, CancellationToken ct = default);
    Task<Product?> UpdateAsync(int id, Product updated, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Decrements the product stock by <paramref name="quantity"/>.
    /// Returns false if the product does not exist.
    /// </summary>
    Task<bool> DecrementStockAsync(int id, int quantity, CancellationToken ct = default);
}
