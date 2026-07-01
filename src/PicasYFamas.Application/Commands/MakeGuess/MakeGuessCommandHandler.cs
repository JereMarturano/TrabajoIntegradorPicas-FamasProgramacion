using System;
using System.Threading;
using System.Threading.Tasks;
using GameCore;
using MediatR;
using PicasYFamas.Application.DTOs;
using PicasYFamas.Domain.Enums;
using PicasYFamas.Domain.Exceptions;
using PicasYFamas.Domain.Repositories;

namespace PicasYFamas.Application.Commands.MakeGuess;

public class MakeGuessCommandHandler : IRequestHandler<MakeGuessCommand, GuessResultDto>
{
    private readonly IGameRepository _gameRepository;

    public MakeGuessCommandHandler(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public async Task<GuessResultDto> Handle(MakeGuessCommand request, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetByIdAsync(request.GameId, cancellationToken);

        if (game == null)
            throw new DomainException("Game not found.");

        if (game.Status != GameStatus.InProgress)
            throw new DomainException($"Game is already finished.");

        // Validate and register the guess via domain (validates format, tracks attempt count/status)
        game.MakeGuess(request.AttemptedNumber);

        // Use ESCMB.GameCore for the official picas/famas calculation and message
        var result = Evaluator.ValidateAttempt(game.SecretNumberValue, request.AttemptedNumber);

        await _gameRepository.UpdateAsync(game, cancellationToken);

        bool isFinished = game.Status != GameStatus.InProgress;

        return new GuessResultDto(result.Pica, result.Fama, result.Message, isFinished);
    }
}

