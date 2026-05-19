using EcomApi.Domain.Enums;

namespace EcomApi.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime OrderedAt { get; set; } = DateTime.UtcNow;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; set; }

    public User? User { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
