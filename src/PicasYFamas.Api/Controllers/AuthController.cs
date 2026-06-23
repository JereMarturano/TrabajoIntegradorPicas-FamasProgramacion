using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PicasYFamas.Application.Commands.Auth;
using PicasYFamas.Application.DTOs;
using PicasYFamas.Application.Responses;

namespace PicasYFamas.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Register([FromBody] RegisterCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(ApiResponse<AuthResponse>.Ok(response));
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login([FromBody] LoginCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(ApiResponse<AuthResponse>.Ok(response));
    }
}
