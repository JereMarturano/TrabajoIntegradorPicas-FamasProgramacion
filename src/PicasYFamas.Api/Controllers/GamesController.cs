using System;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PicasYFamas.Application.Commands.CreateGame;
using PicasYFamas.Application.Commands.MakeGuess;
using PicasYFamas.Application.DTOs;
using PicasYFamas.Application.Queries.GetGame;
using PicasYFamas.Application.Queries.GetGuesses;
using PicasYFamas.Application.Responses;

namespace PicasYFamas.Api.Controllers;

[ApiController]
[Route("api/game/v1")]
public class GamesController : ControllerBase
{
    private readonly IMediator _mediator;

    public GamesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Inicia un nuevo juego. Requiere token JWT.</summary>
    [Authorize]
    [HttpPost("start")]
    public async Task<ActionResult<ApiResponse<StartGameResponseDto>>> StartGame()
    {
        var playerIdClaim = User.FindFirst("playerId")?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (playerIdClaim == null || !Guid.TryParse(playerIdClaim, out var playerId))
            return Unauthorized();

        var result = await _mediator.Send(new CreateGameCommand(playerId));
        return Ok(ApiResponse<StartGameResponseDto>.Ok(result));
    }

    /// <summary>Realiza un intento de adivinar el número secreto. Requiere token JWT.</summary>
    [Authorize]
    [HttpPost("guess")]
    public async Task<ActionResult<ApiResponse<GuessResultDto>>> Guess([FromBody] GuessRequest request)
    {
        var result = await _mediator.Send(new MakeGuessCommand(request.GameId, request.AttemptedNumber));
        return Ok(ApiResponse<GuessResultDto>.Ok(result));
    }

    /// <summary>Consulta el estado de un juego (no requerido por enunciado, útil para debug).</summary>
    [Authorize]
    [HttpGet("{gameId:guid}")]
    public async Task<ActionResult<ApiResponse<GameDto>>> GetGame(Guid gameId)
    {
        var game = await _mediator.Send(new GetGameQuery(gameId));
        return Ok(ApiResponse<GameDto>.Ok(game));
    }

    /// <summary>Retorna todos los intentos de un juego.</summary>
    [Authorize]
    [HttpGet("{gameId:guid}/guesses")]
    public async Task<ActionResult<ApiResponse<System.Collections.Generic.IEnumerable<GuessDto>>>> GetGuesses(Guid gameId)
    {
        var guesses = await _mediator.Send(new GetGuessesQuery(gameId));
        return Ok(ApiResponse<System.Collections.Generic.IEnumerable<GuessDto>>.Ok(guesses));
    }
}

public record GuessRequest(Guid GameId, string AttemptedNumber);

