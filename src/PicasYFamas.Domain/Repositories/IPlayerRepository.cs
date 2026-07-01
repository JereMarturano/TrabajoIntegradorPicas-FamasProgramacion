using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PicasYFamas.Domain.Entities;

namespace PicasYFamas.Domain.Repositories;

public interface IPlayerRepository
{
    Task<Player?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Player?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AddAsync(Player player, CancellationToken cancellationToken = default);
}
