using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configurations;

public class ConfederationConfiguration : IEntityTypeConfiguration<Confederation>
{
    public void Configure(EntityTypeBuilder<Confederation> builder)
    {
        builder.HasKey(c => c.ConfederationId);
        builder.HasIndex(c => c.Name).IsUnique();
        builder.Property(c => c.Name).IsRequired();
        builder.Property(c => c.Slots).IsRequired();

        builder.Ignore(c => c.IsFull);
        builder.Ignore(c => c.RemainingSlots);
        builder.HasMany(c => c.Teams).WithOne().OnDelete(DeleteBehavior.NoAction);
    }
}