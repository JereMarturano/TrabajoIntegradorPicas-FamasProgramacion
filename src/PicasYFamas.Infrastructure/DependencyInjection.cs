using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PicasYFamas.Application.Interfaces;
using PicasYFamas.Domain.Repositories;
using PicasYFamas.Infrastructure.Authentication;
using PicasYFamas.Infrastructure.Persistence;
using PicasYFamas.Infrastructure.Repositories;
using PicasYFamas.Infrastructure.Services;

namespace PicasYFamas.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
