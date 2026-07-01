using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PicasYFamas.Domain.Entities;

namespace PicasYFamas.Infrastructure.Persistence.Configurations;

public class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.ToTable("Players");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Lastname)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Firstname)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Age)
            .IsRequired();

        builder.Property(p => p.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(p => p.Email)
            .IsUnique();

        builder.Property(p => p.PasswordHash)
            .IsRequired();

        builder.HasMany(p => p.Games)
            .WithOne()
            .HasForeignKey(g => g.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
