using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PicasYFamas.Application.DTOs;
using PicasYFamas.Application.Interfaces;
using PicasYFamas.Domain.Exceptions;
using PicasYFamas.Domain.Repositories;

namespace PicasYFamas.Application.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher _passwordHasher;

    public LoginCommandHandler(IPlayerRepository playerRepository, IJwtService jwtService, IPasswordHasher passwordHasher)
    {
        _playerRepository = playerRepository;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var player = await _playerRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (player == null || !_passwordHasher.Verify(request.Password, player.PasswordHash))
            throw new DomainException("Invalid email or password.");

        var token = _jwtService.GenerateToken(player.Id, player.Email);

        return new AuthResponseDto(player.Id, token);
    }
}

