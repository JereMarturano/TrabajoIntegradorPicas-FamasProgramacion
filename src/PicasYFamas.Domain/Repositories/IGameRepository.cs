using System;
using System.Threading;
using System.Threading.Tasks;
using PicasYFamas.Domain.Entities;

namespace PicasYFamas.Domain.Repositories;

public interface IGameRepository
{
    Task<Game?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Game game, CancellationToken cancellationToken = default);
    Task UpdateAsync(Game game, CancellationToken cancellationToken = default);
    Task DeleteAsync(Game game, CancellationToken cancellationToken = default);
}
