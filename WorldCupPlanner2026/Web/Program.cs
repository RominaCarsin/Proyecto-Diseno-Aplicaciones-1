using Web.Components;
using IServices;
using Services;
using IRepository;
using Repository;
using Blazored.LocalStorage;
using Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<IRepository<Stadium>, Repository<Stadium>>();
builder.Services.AddScoped<IStadiumService, StadiumService>();

builder.Services.AddScoped<IRepository<Team>, Repository<Team>>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<ITeamImportService, TeamImportService>();
builder.Services.AddScoped<IConfederationService, ConfederationService>();

builder.Services.AddScoped<IRepository<User>, Repository<User>>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthStateService, AuthStateService>();
builder.Services.AddScoped<ISessionService, SessionService>();

builder.Services.AddScoped<IRepository<AuditLog>, Repository<AuditLog>>();
builder.Services.AddScoped<IAuditService, AuditService>();

builder.Services.AddScoped<IRepository<Match>, Repository<Match>>();
builder.Services.AddScoped<IMatchService, MatchService>();

builder.Services.AddScoped<IRepository<Group>, Repository<Group>>();
builder.Services.AddScoped<IGroupService, GroupService>();

builder.Services.AddScoped<IRepository<Pot>, Repository<Pot>>();
builder.Services.AddScoped<IPotService, PotService>();

builder.Services.AddScoped<IFixtureService, FixtureService>();
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
