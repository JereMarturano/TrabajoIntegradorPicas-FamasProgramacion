using MediatR;
using PicasYFamas.Application.DTOs;

namespace PicasYFamas.Application.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponseDto>;
