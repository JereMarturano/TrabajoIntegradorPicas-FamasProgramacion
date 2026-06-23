using System;
using MediatR;
using PicasYFamas.Application.DTOs;

namespace PicasYFamas.Application.Commands.CreateGame;

public record CreateGameCommand() : IRequest<GameDto>;
