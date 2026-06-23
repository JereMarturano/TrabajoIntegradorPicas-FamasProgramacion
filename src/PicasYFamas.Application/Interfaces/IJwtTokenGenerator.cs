using PicasYFamas.Domain.Entities;

namespace PicasYFamas.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
