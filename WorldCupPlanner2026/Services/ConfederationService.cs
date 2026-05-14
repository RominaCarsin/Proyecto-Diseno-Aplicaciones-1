using Domain;
using DTOs;
using IServices;
using Services.Mappers;

namespace Services;

public class ConfederationService : IConfederationService
{
    private const string ConfederationNotFoundErrorMessage = "Confederation '{0}' not found.";
    private const string ConfederationFullErrorMessage = "Confederation '{0}' is full.";

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
        var confederation = FindConfederationByNameOrThrow(team.ConfederationName);

        confederation.AddTeam(team);
    }

    public void RemoveTeamFromConfederation(Team team)
    {
        var confederation = FindConfederationByNameOrThrow(team.ConfederationName);

        confederation.RemoveTeam(team);
    }

    public void UpdateTeamConfederation(Team team, string newConfederationName)
    {
        bool isAlreadyInConfederation = team.ConfederationName.Equals(
            newConfederationName,
            StringComparison.OrdinalIgnoreCase);

        if (isAlreadyInConfederation)
            return;

        var newConfederation = FindConfederationByNameOrThrow(newConfederationName);

        EnsureConfederationIsNotFull(newConfederation);

        var oldConfederation = FindConfederationByNameOrThrow(team.ConfederationName);

        oldConfederation.RemoveTeam(team);
        team.UpdateConfederationName(newConfederationName);
        newConfederation.AddTeam(team);
    }
    
    private Confederation FindConfederationByNameOrThrow(string name) =>
        _confederations.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
        ?? throw new ArgumentException(string.Format(ConfederationNotFoundErrorMessage, name));

    private static void EnsureConfederationIsNotFull(Confederation confederation)
    {
        if (confederation.IsFull)
            throw new ArgumentException(
                string.Format(ConfederationFullErrorMessage, confederation.Name));
    }
}