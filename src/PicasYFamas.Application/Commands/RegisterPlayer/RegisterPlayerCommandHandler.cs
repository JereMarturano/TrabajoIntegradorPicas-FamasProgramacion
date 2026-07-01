using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PicasYFamas.Application.DTOs;
using PicasYFamas.Application.Interfaces;
using PicasYFamas.Domain.Entities;
using PicasYFamas.Domain.Exceptions;
using PicasYFamas.Domain.Repositories;

namespace PicasYFamas.Application.Commands.RegisterPlayer;

public class RegisterPlayerCommandHandler : IRequestHandler<RegisterPlayerCommand, AuthResponseDto>
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterPlayerCommandHandler(IPlayerRepository playerRepository, IJwtService jwtService, IPasswordHasher passwordHasher)
    {
        _playerRepository = playerRepository;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthResponseDto> Handle(RegisterPlayerCommand request, CancellationToken cancellationToken)
    {
        var existing = await _playerRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existing != null)
            throw new DomainException("Email already registered.");

        var passwordHash = _passwordHasher.Hash(request.Password);

        var player = new Player(
            request.Lastname,
            request.Firstname,
            request.Age,
            request.Email,
            passwordHash);

        await _playerRepository.AddAsync(player, cancellationToken);

        var token = _jwtService.GenerateToken(player.Id, player.Email);

        return new AuthResponseDto(player.Id, token);
    }
}
