using System.Threading;
using System.Threading.Tasks;
using MediatR;
using System.UnauthorizedAccessException;
using PicasYFamas.Application.DTOs;
using PicasYFamas.Application.Interfaces;
using PicasYFamas.Domain.Exceptions;
using PicasYFamas.Domain.Repositories;

namespace PicasYFamas.Application.Queries.GetGame;

public class GetGameQueryHandler : IRequestHandler<GetGameQuery, GameDto>
{
    private readonly IGameRepository _gameRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetGameQueryHandler(IGameRepository gameRepository, ICurrentUserService currentUserService)
    {
        _gameRepository = gameRepository;
        _currentUserService = currentUserService;
    }

    public async Task<GameDto> Handle(GetGameQuery request, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetByIdAsync(request.GameId, cancellationToken);
        if (game == null)
            throw new DomainException("Game not found."); // Return 404 handled by middleware

        if (game.UserId != _currentUserService.UserId)
            throw new UnauthorizedAccessException("You are not allowed to access this game.");

        return new GameDto(game.Id, game.Status.ToString(), game.Guesses.Count, game.CreatedAt);
    }
}
