using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PicasYFamas.Application.DTOs;
using PicasYFamas.Domain.Exceptions;
using PicasYFamas.Domain.Repositories;

namespace PicasYFamas.Application.Queries.GetGame;

public class GetGameQueryHandler : IRequestHandler<GetGameQuery, GameDto>
{
    private readonly IGameRepository _gameRepository;

    public GetGameQueryHandler(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public async Task<GameDto> Handle(GetGameQuery request, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetByIdAsync(request.GameId, cancellationToken);
        if (game == null)
            throw new DomainException("Game not found."); // Return 404 handled by middleware

        return new GameDto(game.Id, game.Status.ToString(), game.Guesses.Count, game.CreatedAt);
    }
}
