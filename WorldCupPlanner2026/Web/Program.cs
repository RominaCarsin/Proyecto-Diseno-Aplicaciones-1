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
builder.Services.AddScoped<IUpdateRankingService, UpdateRankingService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IKnockoutMatchFactory>(sp =>
{
    var stadiumRepo = sp.GetRequiredService<IRepository<Stadium>>();
    var stadiums = stadiumRepo.GetAll().ToList();
    return new KnockoutMatchFactory(stadiums, new DateTime(2026, 7, 1));
});

builder.Services.AddBlazoredLocalStorage();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (app.Environment.IsDevelopment())
        dbContext.Database.EnsureDeleted();

    dbContext.Database.Migrate();

    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    AppDbContext.Seed(dbContext, hasher);
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
