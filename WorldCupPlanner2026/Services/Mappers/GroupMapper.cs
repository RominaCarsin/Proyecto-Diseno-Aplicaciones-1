using Domain;
using DTOs;


namespace Services.Mappers;

public class GroupMapper
{
    
    public static Group ToEntity(GroupRequest request)
    {
        return new Group(request.Label);
    }

    public static GroupResponse FromEntity(Group group)
    {
        return new GroupResponse
        {
            Label = group.Label,
            Teams = group.Teams.Select(TeamMapper.FromEntity).ToList(),
            Matches = group.Matches.Select(MatchMapper.ToResponse).ToList()

        };
    }
}