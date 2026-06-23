using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PicasYFamas.Application.DTOs;
using PicasYFamas.Domain.Enums;
using PicasYFamas.Domain.Exceptions;
using PicasYFamas.Domain.Repositories;

namespace PicasYFamas.Application.Queries.GetGuesses;

public class GetGuessesQueryHandler : IRequestHandler<GetGuessesQuery, IEnumerable<GuessDto>>
{
    private readonly IGameRepository _gameRepository;

    public GetGuessesQueryHandler(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public async Task<IEnumerable<GuessDto>> Handle(GetGuessesQuery request, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetByIdAsync(request.GameId, cancellationToken);
        if (game == null)
            throw new DomainException("Game not found.");

        return game.Guesses.Select(g => new GuessDto(
            g.Number,
            g.Picas,
            g.Famas,
            g.AttemptNumber,
            game.Status == GameStatus.Won && g.Famas == 4
        )).ToList();
    }
}
