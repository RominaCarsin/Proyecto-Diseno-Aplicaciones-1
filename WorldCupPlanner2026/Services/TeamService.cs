using Domain;
using DTOs;
using IServices;
using IRepository;
using Services.Mappers;

namespace Services;

public class TeamService : ITeamService
{
     private readonly IRepository<Team> _repository;
    private readonly IConfederationService _confederationService;

    public TeamService(IRepository<Team> repository, IConfederationService confederationService)
    {
        _repository = repository;
        _confederationService = confederationService;
    }

    public void AddTeam(TeamRequest request)
    {
        CheckNameIsAvailable(request.Name);
        var team = TeamMapper.ToEntity(request);
        _confederationService.AddTeamToConfederation(team);
        _repository.Add(team);
    }

    public void UpdateTeam(string currentName, TeamRequest request)
    {
        var team = GetExistingTeamOrThrowArgumentException(currentName);
        CheckNameIsAvailableForUpdate(currentName, request.Name);

        team.UpdateName(request.Name);
        team.UpdateRankingFifa(request.RankingFifa);

        if (team.ConfederationName != request.ConfederationName)
        {
            team.UpdateConfederationName(request.ConfederationName);
            _confederationService.UpdateTeamConfederation(team, request.ConfederationName);
        }
    }

    public void DeleteTeam(string name)
    {
        var team = GetExistingTeamOrThrowArgumentException(name);
        _confederationService.RemoveTeamFromConfederation(team);
        _repository.Delete(team);
    }

    public IEnumerable<TeamResponse> GetTeams() =>
        _repository.GetAll().Select(TeamMapper.FromEntity);

    private Team? FindByName(string name) =>
        _repository.GetAll().FirstOrDefault(t => t.Name == Team.Normalized(name));

    private Team GetExistingTeamOrThrowArgumentException(string name) =>
        FindByName(name) ?? throw new ArgumentException("Team not found.");

    private void CheckNameIsAvailable(string name)
    {
        if (FindByName(name) != null)
            throw new ArgumentException("A team with this name already exists.");
    }

    private void CheckNameIsAvailableForUpdate(string currentName, string newName)
    {
        var teamWithNewName = FindByName(newName);
        if (teamWithNewName != null && teamWithNewName.Name != currentName)
            throw new ArgumentException("A team with this name already exists.");
    }
}