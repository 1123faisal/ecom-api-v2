using System.Security.Claims;
using EcomApi.Application.DTOs.Order;
using EcomApi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcomApi.API.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _service;

    public OrdersController(IOrderService service)
    {
        _service = service;
    }

    /// <summary>Admin-only: returns all orders in the system.</summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(List<OrderResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<OrderResponseDto>>> GetAll(CancellationToken ct)
    {
        return Ok(await _service.GetAllAsync(ct));
    }

    [HttpGet("my")]
    [ProducesResponseType(typeof(List<OrderResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<OrderResponseDto>>> GetMyOrders(CancellationToken ct)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        return Ok(await _service.GetByUserIdAsync(userId.Value, ct));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponseDto>> GetById(int id, CancellationToken ct)
    {
        var order = await _service.GetByIdAsync(id, ct);
        if (order == null)
            return NotFound($"Order {id} not found.");

        var userId = GetUserId();
        var role = GetUserRole();

        if (role != "Admin" && order.UserId != userId)
            return Forbid();

        return Ok(order);
    }

    [HttpPost]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<OrderResponseDto>> Create(
        CreateOrderDto dto,
        CancellationToken ct
    )
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var order = await _service.CreateAsync(userId.Value, dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponseDto>> UpdateStatus(
        int id,
        UpdateOrderStatusDto dto,
        CancellationToken ct
    )
    {
        var order = await _service.UpdateStatusAsync(id, dto, ct);
        if (order == null)
            return NotFound($"Order {id} not found.");

        return Ok(order);
    }

    private int? GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out var id) ? id : null;
    }

    private string? GetUserRole() => User.FindFirst(ClaimTypes.Role)?.Value;
}

