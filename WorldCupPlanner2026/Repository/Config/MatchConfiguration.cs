using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configurations;

public class MatchConfiguration : IEntityTypeConfiguration<Match>
{
    public void Configure(EntityTypeBuilder<Match> builder)
    {
        
        builder.HasKey(m => m.MatchId);
        builder.Property(m => m.Date).IsRequired();
        builder.Property(m => m.Phase).IsRequired(); //Usa Enums checkear si no debe tener HasConversion<string>();
        builder.Property(m => m.Status).IsRequired(); //Usa Enums checkear si no debe tener HasConversion<string>();
       
        builder.HasOne(m => m.Stadium).WithMany().HasForeignKey(m => m.StadiumId).IsRequired().OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(m => m.HomeTeam).WithMany().HasForeignKey(m => m.HomeTeamId).IsRequired().OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(m => m.AwayTeam).WithMany().HasForeignKey(m => m.AwayTeamId).IsRequired().OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(m => m.Winner).WithMany().HasForeignKey(m => m.WinnerId).OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<Group>().WithMany(g => g.Matches).HasForeignKey(m => m.GroupId).OnDelete(DeleteBehavior.NoAction);
    }
}