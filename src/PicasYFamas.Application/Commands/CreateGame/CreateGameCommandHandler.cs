using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PicasYFamas.Application.DTOs;
using PicasYFamas.Application.Interfaces;
using PicasYFamas.Domain.Entities;
using PicasYFamas.Domain.Repositories;

namespace PicasYFamas.Application.Commands.CreateGame;

public class CreateGameCommandHandler : IRequestHandler<CreateGameCommand, GameDto>
{
    private readonly IGameRepository _gameRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateGameCommandHandler(IGameRepository gameRepository, ICurrentUserService currentUserService)
    {
        _gameRepository = gameRepository;
        _currentUserService = currentUserService;
    }

    public async Task<GameDto> Handle(CreateGameCommand request, CancellationToken cancellationToken)
    {
        var game = new Game(_currentUserService.UserId);
        await _gameRepository.AddAsync(game, cancellationToken);
        
        return new GameDto(game.Id, game.Status.ToString(), game.Guesses.Count, game.CreatedAt);
    }
}
