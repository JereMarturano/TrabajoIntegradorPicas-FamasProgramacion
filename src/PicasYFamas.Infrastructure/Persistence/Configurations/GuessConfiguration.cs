using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PicasYFamas.Domain.Entities;

namespace PicasYFamas.Infrastructure.Persistence.Configurations;

public class GuessConfiguration : IEntityTypeConfiguration<Guess>
{
    public void Configure(EntityTypeBuilder<Guess> builder)
    {
        builder.ToTable("Guesses");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Number)
            .IsRequired()
            .HasMaxLength(4);
    }
}
