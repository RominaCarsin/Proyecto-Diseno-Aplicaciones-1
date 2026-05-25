using Domain;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        this.Database.Migrate();
    }
    
    public DbSet<Team> Teams { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<Confederation> Confederations { get; set; }
    public DbSet<Match> Matches { get; set; }
    public DbSet<Pot>  Pots { get; set; }
    public DbSet<Stadium> Stadiums { get; set; }
    public DbSet<Knockout>  Knockouts { get; set; }
    public DbSet<User> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuramos la relación con Stadium
        modelBuilder.Entity<Match>()
            .HasOne(m => m.Stadium)
            .WithMany()
            .HasForeignKey(m => m.StadiumId) // ✅ Ahora es una propiedad directa
            .OnDelete(DeleteBehavior.NoAction);

        // Configuramos las relaciones con Teams
        modelBuilder.Entity<Match>()
            .HasOne(m => m.HomeTeam)
            .WithMany()
            .HasForeignKey(m => m.HomeTeamId) // ✅ Ahora es una propiedad directa
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Match>()
            .HasOne(m => m.AwayTeam)
            .WithMany()
            .HasForeignKey(m => m.AwayTeamId) // ✅ Ahora es una propiedad directa
            .OnDelete(DeleteBehavior.NoAction);
    }
}