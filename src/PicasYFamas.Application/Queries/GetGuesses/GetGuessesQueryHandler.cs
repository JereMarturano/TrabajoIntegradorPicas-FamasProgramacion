using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using System.UnauthorizedAccessException;
using PicasYFamas.Application.DTOs;
using PicasYFamas.Application.Interfaces;
using PicasYFamas.Domain.Enums;
using PicasYFamas.Domain.Exceptions;
using PicasYFamas.Domain.Repositories;

namespace PicasYFamas.Application.Queries.GetGuesses;

public class GetGuessesQueryHandler : IRequestHandler<GetGuessesQuery, IEnumerable<GuessDto>>
{
    private readonly IGameRepository _gameRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetGuessesQueryHandler(IGameRepository gameRepository, ICurrentUserService currentUserService)
    {
        _gameRepository = gameRepository;
        _currentUserService = currentUserService;
    }

    public async Task<IEnumerable<GuessDto>> Handle(GetGuessesQuery request, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetByIdAsync(request.GameId, cancellationToken);
        if (game == null)
            throw new DomainException("Game not found.");

        if (game.UserId != _currentUserService.UserId)
            throw new UnauthorizedAccessException("You are not allowed to access this game.");

        return game.Guesses.Select(g => new GuessDto(
            g.Number,
            g.Picas,
            g.Famas,
            g.AttemptNumber,
            game.Status == GameStatus.Won && g.Famas == 4
        )).ToList();
    }
}
