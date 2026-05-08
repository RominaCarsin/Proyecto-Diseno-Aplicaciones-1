using Domain;
using DTOs;


namespace Services.Mappers;

public  class GroupMapper
{
    
    public static Group ToEntity(GroupRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        return new Group(request.Label);
    }

    public static GroupResponse FromEntity(Group group)
    {
        if (group == null)
            throw new ArgumentNullException(nameof(group));
        return new GroupResponse
        {
            Label = group.Label,
            Teams = group.Teams.Select(TeamMapper.FromEntity).ToList()
        };
    }
}