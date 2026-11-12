using GestionCommerciale.Api.Dtos;
using GestionCommerciale.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionCommerciale.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginDto dto)
    {
        var response = await _authService.LoginAsync(dto);

        if (response is null)
        {
            return Unauthorized(new { status = 401, message = "Identifiant ou mot de passe incorrect." });
        }

        return Ok(response);
    }
}