using EcomApi.Application.DTOs.Order;
using EcomApi.Application.Services.Interfaces;
using EcomApi.Domain.Entities;
using EcomApi.Domain.Enums;
using EcomApi.Domain.Exceptions;
using EcomApi.Domain.Interfaces;

namespace EcomApi.Application.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;

    public OrderService(IProductRepository productRepository, IOrderRepository orderRepository)
    {
        _productRepository = productRepository;
        _orderRepository = orderRepository;
    }

    public async Task<OrderResponseDto> CreateAsync(int userId, CreateOrderDto dto, CancellationToken ct = default)
    {
        var orderItems = new List<OrderItem>();
        decimal totalAmount = 0;

        foreach (var item in dto.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId, ct);
            if (product == null)
                throw new NotFoundException($"Product {item.ProductId} not found.");

            if (product.Stock < item.Quantity)
                throw new BusinessRuleException(
                    $"Insufficient stock for '{product.Name}'. Available: {product.Stock}, Requested: {item.Quantity}."
                );

            orderItems.Add(
                new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price,
                }
            );

            totalAmount += product.Price * item.Quantity;
            await _productRepository.DecrementStockAsync(item.ProductId, item.Quantity, ct);
        }

        var order = new Order
        {
            UserId = userId,
            TotalAmount = totalAmount,
            Status = OrderStatus.Pending,
            OrderItems = orderItems,
        };

        var created = await _orderRepository.CreateAsync(order, ct);
        return MapToDto(created);
    }

    public async Task<List<OrderResponseDto>> GetAllAsync(CancellationToken ct = default)
    {
        var orders = await _orderRepository.GetAllAsync(ct);
        return orders.Select(MapToDto).ToList();
    }

    public async Task<OrderResponseDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, ct);
        return order == null ? null : MapToDto(order);
    }

    public async Task<List<OrderResponseDto>> GetByUserIdAsync(int userId, CancellationToken ct = default)
    {
        var orders = await _orderRepository.GetByUserIdAsync(userId, ct);
        return orders.Select(MapToDto).ToList();
    }

    public async Task<OrderResponseDto?> UpdateStatusAsync(int id, UpdateOrderStatusDto dto, CancellationToken ct = default)
    {
        var order = await _orderRepository.UpdateStatusAsync(id, dto.Status, ct);
        return order == null ? null : MapToDto(order);
    }

    private static OrderResponseDto MapToDto(Order order) =>
        new(
            order.Id,
            order.UserId,
            order.User?.Username ?? "Unknown",
            order.OrderedAt,
            order.Status.ToString(),
            order.TotalAmount,
            order.OrderItems.Select(MapToOrderItemDto).ToList()
        );

    private static OrderItemResponseDto MapToOrderItemDto(OrderItem oi) =>
        new(
            oi.ProductId,
            oi.Product?.Name ?? "Unknown",
            oi.Quantity,
            oi.UnitPrice,
            oi.Quantity * oi.UnitPrice
        );
}

