using System;
using MediatR;
using PicasYFamas.Application.DTOs;

namespace PicasYFamas.Application.Commands.MakeGuess;

public record MakeGuessCommand(Guid GameId, string AttemptedNumber) : IRequest<GuessResultDto>;

