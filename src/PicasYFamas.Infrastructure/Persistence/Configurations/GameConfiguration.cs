using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PicasYFamas.Domain.Entities;

namespace PicasYFamas.Infrastructure.Persistence.Configurations;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.ToTable("Games");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.SecretNumberValue)
            .IsRequired()
            .HasMaxLength(4);

        builder.Property(g => g.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(20);

        builder.HasMany(g => g.Guesses)
            .WithOne()
            .HasForeignKey("GameId") // Shadow property
            .OnDelete(DeleteBehavior.Cascade);
    }
}
