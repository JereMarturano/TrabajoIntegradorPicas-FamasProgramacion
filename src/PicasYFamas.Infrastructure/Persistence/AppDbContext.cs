using Microsoft.EntityFrameworkCore;
using PicasYFamas.Domain.Entities;

namespace PicasYFamas.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Game> Games => Set<Game>();
    public DbSet<Guess> Guesses => Set<Guess>();
    public DbSet<User> Users => Set<User>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
