using System;

namespace EcomApi.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public double Price { get; set; }
    public int Stock { get; set; }
    public int CategoryId { get; set; }

    public Category? Category { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
