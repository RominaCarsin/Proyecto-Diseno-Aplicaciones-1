using Domain;
using DTOs;
using IRepository;
using IServices;
using Services.Mappers;

namespace Services;

public class GroupService : IGroupService
{
    private readonly IRepository<Group> _repository;
    private const string GroupNotFoundErrorMessage = "Group was not found.";

    public GroupService(IRepository<Group> repository)
    {
        _repository = repository;
    }

    public void AddGroup(GroupRequest request)
    {
        Group group = GroupMapper.ToEntity(request);

        _repository.Add(group);
    }

    public IEnumerable<GroupResponse> GetGroups()
    {
        return _repository
            .GetAll()
            .Select(GroupMapper.FromEntity);
    }

    public void DeleteGroup(char label)
    {
        Domain.Group group = _repository
            .GetAll()
            .First(g => g.Label == label);

        _repository.Delete(group);
    }

    public void AddTeamToGroup(char groupLabel, TeamRequest teamRequest)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<TeamResponse> GetGroupStandings(char groupLabel)
    {
        throw new NotImplementedException();
    }

    public TeamResponse GetTeamByPosition(char groupLabel, int position)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<MatchResponse> GetGroupMatches(char groupLabel)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<MatchResponse> GetPendingGroupMatches(char groupLabel)
    {
        throw new NotImplementedException();
    }
    
    public GroupResponse GetGroupByLabel(char label)
    {
        var group = _repository
            .GetAll()
            .First(g => g.Label == label);

        return GroupMapper.FromEntity(group);
    }
}