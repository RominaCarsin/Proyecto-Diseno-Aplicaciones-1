using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configurations;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.HasKey(t => t.TeamId);
        builder.HasIndex(t => t.Name).IsUnique();
        builder.Property(t => t.Name).IsRequired().HasMaxLength(60);
        builder.Property(t => t.ConfederationName).IsRequired();
        builder.Property(t => t.RankingFifa).IsRequired();
        builder.Property(t => t.GroupLabel).HasMaxLength(1);
    }
}