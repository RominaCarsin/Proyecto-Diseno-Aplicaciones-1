using Domain;
using DTOs;
using IRepository;
using IServices;
using Services.Mappers;

namespace Services;

public class GroupService : IGroupService
{
    private readonly IRepository<Group> _repository;

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

    public GroupResponse GetGroupByLabel(char label)
    {
        throw new NotImplementedException();
    }

    public void DeleteGroup(char label)
    {
        throw new NotImplementedException();
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
}