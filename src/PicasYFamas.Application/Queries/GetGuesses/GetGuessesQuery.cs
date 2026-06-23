using System;
using System.Collections.Generic;
using MediatR;
using PicasYFamas.Application.DTOs;

namespace PicasYFamas.Application.Queries.GetGuesses;

public record GetGuessesQuery(Guid GameId) : IRequest<IEnumerable<GuessDto>>;
