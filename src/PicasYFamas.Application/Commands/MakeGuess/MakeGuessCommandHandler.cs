using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PicasYFamas.Application.DTOs;
using PicasYFamas.Domain.Enums;
using PicasYFamas.Domain.Exceptions;
using PicasYFamas.Domain.Repositories;

namespace PicasYFamas.Application.Commands.MakeGuess;

public class MakeGuessCommandHandler : IRequestHandler<MakeGuessCommand, GuessDto>
{
    private readonly IGameRepository _gameRepository;

    public MakeGuessCommandHandler(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public async Task<GuessDto> Handle(MakeGuessCommand request, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetByIdAsync(request.GameId, cancellationToken);
        
        if (game == null)
            throw new DomainException("Game not found."); // In a real app we might throw a NotFoundException

        var guess = game.MakeGuess(request.Number);
        
        await _gameRepository.UpdateAsync(game, cancellationToken);

        bool isWinner = game.Status == GameStatus.Won;

        return new GuessDto(guess.Number, guess.Picas, guess.Famas, guess.AttemptNumber, isWinner);
    }
}
