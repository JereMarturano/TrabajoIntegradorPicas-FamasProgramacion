using System;
using System.Threading.Tasks;
using Asp.Versioning;
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

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class GamesController : ControllerBase
{
    private readonly IMediator _mediator;

    public GamesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<GameDto>>> CreateGame()
    {
        var game = await _mediator.Send(new CreateGameCommand());
        return CreatedAtAction(nameof(GetGame), new { gameId = game.GameId, version = "1.0" }, ApiResponse<GameDto>.Ok(game));
    }

    [HttpPost("{gameId:guid}/guesses")]
    public async Task<ActionResult<ApiResponse<GuessDto>>> MakeGuess(Guid gameId, [FromBody] MakeGuessRequest request)
    {
        var guess = await _mediator.Send(new MakeGuessCommand(gameId, request.Number));
        return Ok(ApiResponse<GuessDto>.Ok(guess));
    }

    [HttpGet("{gameId:guid}")]
    public async Task<ActionResult<ApiResponse<GameDto>>> GetGame(Guid gameId)
    {
        var game = await _mediator.Send(new GetGameQuery(gameId));
        return Ok(ApiResponse<GameDto>.Ok(game));
    }

    [HttpGet("{gameId:guid}/guesses")]
    public async Task<ActionResult<ApiResponse<object>>> GetGuesses(Guid gameId)
    {
        var guesses = await _mediator.Send(new GetGuessesQuery(gameId));
        return Ok(ApiResponse<object>.Ok(guesses));
    }
}

public record MakeGuessRequest(string Number);
