using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configurations;

public class StadiumConfiguration : IEntityTypeConfiguration<Stadium>
{
    public void Configure(EntityTypeBuilder<Stadium> builder)
    {
        builder.HasKey(s => s.StadiumId);
        builder.Property(s => s.Name).IsRequired().HasMaxLength(80);
        builder.Property(s => s.City).IsRequired().HasMaxLength(60);
        builder.Property(s => s.Description).HasMaxLength(400);
        builder.Property(s => s.Capacity).IsRequired();
    }
}