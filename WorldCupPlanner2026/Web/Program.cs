using Web.Components;
using IServices;
using Services;
using IRepository;
using Repository;
using Blazored.LocalStorage;
using Domain;
using Microsoft.EntityFrameworkCore;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services
    .AddBlazorise(options =>
    {
        options.Immediate = true;
    })
    .AddBootstrap5Providers()
    .AddFontAwesomeIcons();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));


builder.Services.AddScoped<IStadiumService, StadiumService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthStateService, AuthStateService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddScoped<ITournamentService, TournamentService>();
builder.Services.AddScoped<IServices.INotificationService, NotificationService>();
builder.Services.AddScoped<ShuffleService>(_ => new ShuffleService(42));
builder.Services.AddScoped<IShuffleService>(sp => sp.GetRequiredService<ShuffleService>());
builder.Services.AddScoped<IUpdateRankingService, UpdateRankingService>();

builder.Services.AddBlazoredLocalStorage();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (app.Environment.IsDevelopment())
        dbContext.Database.EnsureDeleted();

    dbContext.Database.Migrate();

    if (!dbContext.Users.Any(user => user.Email == "admin@test.com"))
    {
        dbContext.Users.Add(new User(
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
            name: "Periodista",
            surname: "Prensa",
            email: "periodista@test.com",
            dateOfBirth: new DateTime(2000, 1, 1),
            plainPassword: "Password1!",
            roles: Role.Journalist));

        dbContext.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
