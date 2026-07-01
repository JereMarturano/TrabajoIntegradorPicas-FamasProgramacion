using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PicasYFamas.Domain.Entities;
using PicasYFamas.Domain.Repositories;
using PicasYFamas.Infrastructure.Persistence;

namespace PicasYFamas.Infrastructure.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly AppDbContext _context;

    public PlayerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Player?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Players
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Player?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Players
            .FirstOrDefaultAsync(p => p.Email == email.ToLowerInvariant(), cancellationToken);
    }

    public async Task AddAsync(Player player, CancellationToken cancellationToken = default)
    {
        await _context.Players.AddAsync(player, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
