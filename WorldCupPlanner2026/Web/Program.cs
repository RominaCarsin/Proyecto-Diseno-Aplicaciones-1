using Web.Components;
using IServices;
using Services;
using IRepository;
using Repository;
using Blazored.LocalStorage;
using Domain;
using DTOs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IRepository<Stadium>, Repository<Stadium>>();
builder.Services.AddScoped<IStadiumService, StadiumService>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<IRepository<Team>, Repository<Team>>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IConfederationService, ConfederationService>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<IRepository<Match>, Repository<Match>>();
builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddScoped<IRepository<Group>, Repository<Group>>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IFixtureService, FixtureService>();
var app = builder.Build();
// ─── SEEDER TEMPORAL — borrar antes de producción ───
using (var scope = app.Services.CreateScope())
{
    var teamService    = scope.ServiceProvider.GetRequiredService<ITeamService>();
    var stadiumService = scope.ServiceProvider.GetRequiredService<IStadiumService>();
    var fixtureService = scope.ServiceProvider.GetRequiredService<IFixtureService>();
    var groupService   = scope.ServiceProvider.GetRequiredService<IGroupService>();

    try
    {
        if (!groupService.GetGroups().Any())
        {
            for (int i = 1; i <= 4; i++)
                stadiumService.AddStadium(new StadiumRequest
                {
                    Name = $"Estadio {i}",
                    City = "Montevideo",
                    Description = null,
                    Capacity = 50000
                });

            var slots = new Dictionary<string, int>
            {
                { "UEFA", 16 },
                { "CONMEBOL", 7 },
                { "CONCACAF", 7 },
                { "CAF", 9 },
                { "AFC", 8 },
                { "OFC", 1 }
            };

            int ranking = 1500;
            foreach (var (confederation, count) in slots)
            {
                for (int i = 1; i <= count; i++)
                {
                    teamService.AddTeam(new TeamRequest
                    {
                        Name = $"{confederation}_{i:00}",
                        ConfederationName = confederation,
                        RankingFifa = ranking--
                    });
                }
            }

            fixtureService.GenerateFixture(42, new DateTime(2026, 6, 1));
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"SEEDER ERROR: {ex.Message}");
        Console.WriteLine(ex.StackTrace);
    }
}
// ─── FIN SEEDER ───


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();



app.Run();