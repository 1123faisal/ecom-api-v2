using System.Security.Cryptography;
using System.Text;
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
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponseDto>> RegisterAdmin(
        RegisterDto dto,
        [FromHeader(Name = "X-Admin-Secret")] string? adminSecret,
        CancellationToken ct
    )
    {
        // Use constant-time comparison to prevent timing attacks on the admin secret.
        if (!IsValidAdminSecret(adminSecret))
            return Unauthorized("Invalid admin secret.");

        var result = await _authService.RegisterAdminAsync(dto, ct);
        return CreatedAtAction(nameof(Login), result);
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto, CancellationToken ct)
    {
        var result = await _authService.RegisterAsync(dto, ct);
        return CreatedAtAction(nameof(Login), result);
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

    private bool IsValidAdminSecret(string? provided)
    {
        if (provided == null)
            return false;

        var expected = Encoding.UTF8.GetBytes(_adminOptions.Secret);
        var actual = Encoding.UTF8.GetBytes(provided);

        // Pad both to the same length so FixedTimeEquals doesn't short-circuit on length mismatch.
        if (expected.Length != actual.Length)
        {
            // Still perform a dummy comparison to consume similar time.
            CryptographicOperations.FixedTimeEquals(expected, expected);
            return false;
        }

        return CryptographicOperations.FixedTimeEquals(expected, actual);
    }
}
