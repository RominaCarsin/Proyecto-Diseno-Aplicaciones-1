using Domain;
using DTOs;
using IServices;

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
        _confederations.Select(ConfederationResponse.FromEntity);

    public void AddTeamToConfederation(Team team)
    {
        var confederation = _confederations
            .FirstOrDefault(c => c.Name == team.ConfederationName);

        if (confederation == null)
            throw new ArgumentException($"Confederation '{team.ConfederationName}' not found.");

        confederation.AddTeam(team);
    }

    public void RemoveTeamFromConfederation(Team team)
    {
        var confederation = _confederations
            .FirstOrDefault(c => c.Name == team.ConfederationName);

        if (confederation == null)
            throw new ArgumentException($"Confederation '{team.ConfederationName}' not found.");

        confederation.RemoveTeam(team);
    }

    public void UpdateTeamConfederation(Team team, string newConfederationName)
    {
        if (team.ConfederationName == newConfederationName)
            return;

        var newConfederation = _confederations
            .FirstOrDefault(c => c.Name == newConfederationName);
        if (newConfederation == null)
            throw new ArgumentException($"Confederation '{newConfederationName}' not found.");

        if (newConfederation.IsFull)
            throw new ArgumentException($"Confederation '{newConfederationName}' is full.");

        var oldConfederation = _confederations
            .FirstOrDefault(c => c.Name == team.ConfederationName);
        if (oldConfederation == null)
            throw new ArgumentException($"Confederation '{team.ConfederationName}' not found.");

        oldConfederation.RemoveTeam(team);
        team.UpdateConfederationName(newConfederationName);
        newConfederation.AddTeam(team);
    }
}