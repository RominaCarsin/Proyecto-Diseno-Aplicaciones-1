using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.HasKey(g => g.GroupId);
        builder.Property(g => g.Label).IsRequired();
        builder.HasMany(g => g.Teams).WithOne().OnDelete(DeleteBehavior.NoAction);
        builder.HasMany(g => g.Matches).WithOne().HasForeignKey(m => m.GroupId).OnDelete(DeleteBehavior.NoAction);
    }
}