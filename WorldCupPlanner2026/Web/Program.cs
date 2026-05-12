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
builder.Services.AddSingleton<IRepository<Stadium>, Repository<Stadium>>();
builder.Services.AddSingleton<IStadiumService, StadiumService>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<IRepository<Team>, Repository<Team>>();
builder.Services.AddSingleton<ITeamService, TeamService>();
builder.Services.AddSingleton<IConfederationService, ConfederationService>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddSingleton<IRepository<Match>, Repository<Match>>();
builder.Services.AddSingleton<IMatchService, MatchService>();
builder.Services.AddSingleton<IRepository<Group>, Repository<Group>>();

var app = builder.Build();


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