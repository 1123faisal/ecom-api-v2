using System.ComponentModel.DataAnnotations;
using EcomApi.Domain.Enums;

namespace EcomApi.Application.DTOs.Order;

public record CreateOrderItemDto(
    [Required, Range(1, int.MaxValue, ErrorMessage = "ProductId must be a positive integer.")]
        int ProductId,
    [Required, Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100.")] int Quantity
);

public record CreateOrderDto(
    [Required, MinLength(1, ErrorMessage = "Order must contain at least one item.")]
        List<CreateOrderItemDto> Items
);

public record OrderItemResponseDto(
    int ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal Subtotal
);

public record OrderResponseDto(
    int Id,
    int UserId,
    string Username,
    DateTime OrderedAt,
    string Status,
    decimal TotalAmount,
    List<OrderItemResponseDto> Items
);

public record UpdateOrderStatusDto([Required] OrderStatus Status);
