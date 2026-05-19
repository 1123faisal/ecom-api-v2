using EcomApi.Application.DTOs.Category;
using EcomApi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcomApi.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _service;

    public CategoriesController(ICategoryService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<CategoryResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CategoryResponseDto>>> GetAll(CancellationToken ct)
    {
        return Ok(await _service.GetAllAsync(ct));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryResponseDto>> GetById(int id, CancellationToken ct)
    {
        var category = await _service.GetByIdAsync(id, ct);
        if (category == null)
            return NotFound($"Category {id} not found.");

        return Ok(category);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CategoryResponseDto>> Create(
        CreateCategoryDto dto,
        CancellationToken ct
    )
    {
        var created = await _service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryResponseDto>> Update(
        int id,
        UpdateCategoryDto dto,
        CancellationToken ct
    )
    {
        var category = await _service.UpdateAsync(id, dto, ct);
        if (category == null)
            return NotFound($"Category {id} not found.");

        return Ok(category);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        // CategoryService.DeleteAsync throws NotFoundException if not found;
        // the ExceptionMiddleware will convert it to 404.
        await _service.DeleteAsync(id, ct);
        return NoContent();
    }
}

