using EcomApi.Application.DTOs.Auth;
using EcomApi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcomApi.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;

    public AuthController(IAuthService authService, IConfiguration configuration)
    {
        _authService = authService;
        _configuration = configuration;
    }

    [HttpPost("register-admin")]
    public async Task<ActionResult<AuthResponseDto>> RegisterAdmin(
        RegisterDto dto,
        [FromHeader(Name = "X-Admin-Secret")] string? adminSecret
    )
    {
        if (adminSecret != _configuration["AdminSecret"])
            return Unauthorized("Invalid Admin Secret.");

        var result = await _authService.RegisterAdminAsync(dto);
        if (result == null)
            return BadRequest("Username or email already taken");
        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        if (result == null)
            return BadRequest("Username or email already exists.");
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        if (result == null)
            return Unauthorized("Invalid username or password.");
        return Ok(result);
    }
}
