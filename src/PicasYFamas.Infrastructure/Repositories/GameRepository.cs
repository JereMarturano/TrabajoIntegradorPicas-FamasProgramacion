using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PicasYFamas.Domain.Entities;
using PicasYFamas.Domain.Repositories;
using PicasYFamas.Infrastructure.Persistence;

namespace PicasYFamas.Infrastructure.Repositories;

public class GameRepository : IGameRepository
{
    private readonly AppDbContext _context;

    public GameRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Game?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Games
            .Include(g => g.Guesses)
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public async Task AddAsync(Game game, CancellationToken cancellationToken = default)
    {
        await _context.Games.AddAsync(game, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Game game, CancellationToken cancellationToken = default)
    {
        _context.Games.Update(game);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Game game, CancellationToken cancellationToken = default)
    {
        _context.Games.Remove(game);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
