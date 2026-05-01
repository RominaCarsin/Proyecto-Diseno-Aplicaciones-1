using Domain;
using DTOs;
using IServices;
using Services.Mappers;

namespace Services;

public class ConfederationService : IConfederationService
{
    private readonly List<Confederation> _confederations = new()
    {
        new Confederation("UEFA",     16),
        new Confederation("CAF",       9),
        new Confederation("AFC",       8),
        new Confederation("CONCACAF",  7),
        new Confederation("CONMEBOL",  7),
        new Confederation("OFC",       1),
    };

    public IEnumerable<ConfederationResponse> GetAll() => 
        _confederations.Select(ConfederationMapper.FromEntity);

    public void AddTeamToConfederation(Team team)
    {
        var confederation = GetExistingConfederationOrThrowArgumentException(team.ConfederationName);
        CheckConfederationIsNotFull(confederation);
        
        confederation.AddTeam(team);
    }

    public void RemoveTeamFromConfederation(Team team)
    {
        var confederation = GetExistingConfederationOrThrowArgumentException(team.ConfederationName);
        confederation.RemoveTeam(team);
    }

    public void UpdateTeamConfederation(Team team, string newConfederationName)
    {
        if (team.ConfederationName == newConfederationName)
            return;

        var newConfederation = GetExistingConfederationOrThrowArgumentException(newConfederationName);
        CheckConfederationIsNotFull(newConfederation);

        var oldConfederation = GetExistingConfederationOrThrowArgumentException(team.ConfederationName);

        oldConfederation.RemoveTeam(team);
        team.UpdateConfederationName(newConfederationName);
        newConfederation.AddTeam(team);
    }
    

    private Confederation? FindByName(string name) =>
        _confederations.FirstOrDefault(c => c.Name == name);

    private Confederation GetExistingConfederationOrThrowArgumentException(string name) =>
        FindByName(name) ?? throw new ArgumentException($"Confederation '{name}' not found.");

    private void CheckConfederationIsNotFull(Confederation confederation)
    {
        if (confederation.IsFull)
            throw new ArgumentException($"Confederation '{confederation.Name}' is full.");
    }
}