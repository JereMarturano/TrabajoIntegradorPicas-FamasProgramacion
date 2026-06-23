using System;
using System.Threading;
using System.Threading.Tasks;
using PicasYFamas.Domain.Entities;

namespace PicasYFamas.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
}
