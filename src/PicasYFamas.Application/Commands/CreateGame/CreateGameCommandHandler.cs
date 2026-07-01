using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PicasYFamas.Application.DTOs;
using PicasYFamas.Domain.Entities;
using PicasYFamas.Domain.Exceptions;
using PicasYFamas.Domain.Repositories;

namespace PicasYFamas.Application.Commands.CreateGame;

public class CreateGameCommandHandler : IRequestHandler<CreateGameCommand, StartGameResponseDto>
{
    private readonly IGameRepository _gameRepository;
    private readonly IPlayerRepository _playerRepository;

    public CreateGameCommandHandler(IGameRepository gameRepository, IPlayerRepository playerRepository)
    {
        _gameRepository = gameRepository;
        _playerRepository = playerRepository;
    }

    public async Task<StartGameResponseDto> Handle(CreateGameCommand request, CancellationToken cancellationToken)
    {
        var player = await _playerRepository.GetByIdAsync(request.PlayerId, cancellationToken);
        if (player == null)
            throw new DomainException("Player not found.");

        var activeGame = await _gameRepository.GetActiveGameByPlayerAsync(request.PlayerId, cancellationToken);
        if (activeGame != null)
            throw new DomainException("You already have an active game. Finish it before starting a new one.");

        var game = new Game(request.PlayerId);
        await _gameRepository.AddAsync(game, cancellationToken);

        return new StartGameResponseDto(game.Id, game.PlayerId, game.CreatedAt);
    }
}

