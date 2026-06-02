using Web.Components;
using IServices;
using Services;
using IRepository;
using Repository;
using Blazored.LocalStorage;
using Domain;
using Microsoft.EntityFrameworkCore; // Añadido para EF Core

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));


builder.Services.AddScoped<IStadiumService, StadiumService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<ITeamImportService, TeamImportService>();
builder.Services.AddScoped<IConfederationService, ConfederationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthStateService, AuthStateService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<ITournamentService, TournamentService>();
//builder.Services.AddScoped<IMatchService, MatchService>();
//builder.Services.AddScoped<IGroupService, GroupService>();
//builder.Services.AddScoped<IPotService, PotService>();
//builder.Services.AddScoped<IFixtureService, FixtureService>();
builder.Services.AddScoped<ShuffleService>(_ => new ShuffleService(42));
builder.Services.AddScoped<Domain.IShuffleService>(sp => sp.GetRequiredService<ShuffleService>());

builder.Services.AddBlazoredLocalStorage();

var app = builder.Build();

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