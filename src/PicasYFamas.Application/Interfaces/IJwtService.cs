using System;
using System.Threading;
using System.Threading.Tasks;

namespace PicasYFamas.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(Guid playerId, string email);
}
