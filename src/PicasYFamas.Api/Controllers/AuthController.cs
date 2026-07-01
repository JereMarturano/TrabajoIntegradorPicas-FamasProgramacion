using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PicasYFamas.Application.Commands.Login;
using PicasYFamas.Application.Commands.RegisterPlayer;
using PicasYFamas.Application.DTOs;
using PicasYFamas.Application.Responses;

namespace PicasYFamas.Api.Controllers;

[ApiController]
[Route("api/game/v1")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Registra un nuevo jugador y retorna un JWT automáticamente.</summary>
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterRequest request)
    {
        var result = await _mediator.Send(new RegisterPlayerCommand(
            request.Lastname,
            request.Firstname,
            request.Age,
            request.Email,
            request.Password));

        return Ok(ApiResponse<AuthResponseDto>.Ok(result));
    }

    /// <summary>Inicia sesión y retorna un JWT.</summary>
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginRequest request)
    {
        var result = await _mediator.Send(new LoginCommand(request.Email, request.Password));
        return Ok(ApiResponse<AuthResponseDto>.Ok(result));
    }
}

public record RegisterRequest(string Lastname, string Firstname, int Age, string Email, string Password);
public record LoginRequest(string Email, string Password);
