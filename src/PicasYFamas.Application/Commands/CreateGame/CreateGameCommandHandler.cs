using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PicasYFamas.Application.DTOs;
using PicasYFamas.Domain.Entities;
using PicasYFamas.Domain.Repositories;

namespace PicasYFamas.Application.Commands.CreateGame;

public class CreateGameCommandHandler : IRequestHandler<CreateGameCommand, GameDto>
{
    private readonly IGameRepository _gameRepository;

    public CreateGameCommandHandler(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public async Task<GameDto> Handle(CreateGameCommand request, CancellationToken cancellationToken)
    {
        var game = new Game();
        await _gameRepository.AddAsync(game, cancellationToken);
        
        return new GameDto(game.Id, game.Status.ToString(), game.Guesses.Count, game.CreatedAt);
    }
}
