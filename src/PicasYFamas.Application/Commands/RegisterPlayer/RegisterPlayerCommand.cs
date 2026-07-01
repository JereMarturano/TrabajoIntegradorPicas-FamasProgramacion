using MediatR;
using PicasYFamas.Application.DTOs;

namespace PicasYFamas.Application.Commands.RegisterPlayer;

public record RegisterPlayerCommand(
    string Lastname,
    string Firstname,
    int Age,
    string Email,
    string Password) : IRequest<AuthResponseDto>;
