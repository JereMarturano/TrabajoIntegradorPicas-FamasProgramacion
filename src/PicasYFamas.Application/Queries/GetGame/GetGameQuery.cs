using System;
using MediatR;
using PicasYFamas.Application.DTOs;

namespace PicasYFamas.Application.Queries.GetGame;

public record GetGameQuery(Guid GameId) : IRequest<GameDto>;
