using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configurations;

public class PotConfiguration : IEntityTypeConfiguration<Pot>
{
    public void Configure(EntityTypeBuilder<Pot> builder)
    {
        builder.HasKey(p => p.PotId);
        builder.Property(p => p.Number).IsRequired();
        builder.HasMany(p => p.Teams).WithOne().OnDelete(DeleteBehavior.NoAction);
    }
}