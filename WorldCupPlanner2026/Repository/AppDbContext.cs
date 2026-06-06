using Domain;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class AppDbContext : DbContext
{
    public DbSet<Team> Teams { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<Confederation> Confederations { get; set; }
    public DbSet<Match> Matches { get; set; }
    public DbSet<Pot> Pots { get; set; }
    public DbSet<Stadium> Stadiums { get; set; }
    public DbSet<Knockout> Knockouts { get; set; }
    public DbSet<User> Users { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
