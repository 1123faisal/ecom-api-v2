using EcomApi.API.Options;
using EcomApi.Application.DTOs.Auth;
using EcomApi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EcomApi.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly AdminOptions _adminOptions;

    public AuthController(IAuthService authService, IOptions<AdminOptions> adminOptions)
    {
        _authService = authService;
        _adminOptions = adminOptions.Value;
    }

    [HttpPost("register-admin")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponseDto>> RegisterAdmin(
        RegisterDto dto,
        [FromHeader(Name = "X-Admin-Secret")] string? adminSecret,
        CancellationToken ct
    )
    {
        if (adminSecret != _adminOptions.Secret)
            return Unauthorized("Invalid admin secret.");

        var result = await _authService.RegisterAdminAsync(dto, ct);
        if (result == null)
            return Conflict("Username or email is already taken.");

        return Ok(result);
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto, CancellationToken ct)
    {
        var result = await _authService.RegisterAsync(dto, ct);
        if (result == null)
            return Conflict("Username or email already exists.");

        return Ok(result);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto, CancellationToken ct)
    {
        var result = await _authService.LoginAsync(dto, ct);
        if (result == null)
            return Unauthorized("Invalid username or password.");

        return Ok(result);
    }
}

