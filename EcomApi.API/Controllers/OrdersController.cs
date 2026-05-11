using System.Security.Claims;
using EcomApi.Application.DTOs.Order;
using EcomApi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

    [HttpGet]
    public async Task<ActionResult<List<OrderResponseDto>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("my")]
    public async Task<ActionResult<List<OrderResponseDto>>> GetMyOrders()
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();
        return Ok(await _service.GetByUserIdAsync(userId.Value));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderResponseDto?>> GetById(int id)
    {
        var order = await _service.GetByIdAsync(id);
        if (order == null)
            return NotFound($"Order {id} not found");

        var userId = GetUserId();
        var role = GetUserRole();

        if (role != "Admin" && order.UserId != userId)
            return Forbid();

        return Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponseDto>> Create(CreateOrderDto dto)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var order = await _service.CreateAsync(userId.Value, dto);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<OrderResponseDto?>> UpdateStatus(
        int id,
        UpdateOrderStatusDto dto
    )
    {
        var order = await _service.UpdateStatusAsync(id, dto);
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
