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
    public DbSet<User> Users { get; set; }
    public DbSet<MatchIncidence> MatchIncidences { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public static void Seed(AppDbContext dbContext, IPasswordHasher passwordHasher)
    {
        if (!dbContext.Users.Any(user => user.Email == "solo.admin@test.com"))
        {
            dbContext.Users.Add(new User(
                passwordHasher,
                name: "Solo",
                surname: "Admin",
                email: "solo.admin@test.com",
                dateOfBirth: new DateTime(2000, 1, 1),
                plainPassword: "Password1!",
                roles: Role.Admin));

            dbContext.SaveChanges();
        }

        if (!dbContext.Users.Any(user => user.Email == "solo.editor@test.com"))
        {
            dbContext.Users.Add(new User(
                passwordHasher,
                name: "Solo",
                surname: "Editor",
                email: "solo.editor@test.com",
                dateOfBirth: new DateTime(2000, 1, 1),
                plainPassword: "Password1!",
                roles: Role.Editor));

            dbContext.SaveChanges();
        }

        if (!dbContext.Users.Any(user => user.Email == "admin@test.com"))
        {
            dbContext.Users.Add(new User(
                passwordHasher,
                name: "Admin",
                surname: "Principal",
                email: "admin@test.com",
                dateOfBirth: new DateTime(2000, 1, 1),
                plainPassword: "Password1!",
                roles: Role.Admin | Role.Editor));

            dbContext.SaveChanges();
        }

        if (!dbContext.Users.Any(user => user.Email == "periodista@test.com"))
        {
            dbContext.Users.Add(new User(
                passwordHasher,
                name: "Periodista",
                surname: "Prensa",
                email: "periodista@test.com",
                dateOfBirth: new DateTime(2000, 1, 1),
                plainPassword: "Password1!",
                roles: Role.Journalist));

            dbContext.SaveChanges();
        }
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
