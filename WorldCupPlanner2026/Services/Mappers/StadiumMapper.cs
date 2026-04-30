using Domain;
using DTOs;

namespace Services.Mappers;

public static class StadiumMapper
{
    public static Stadium ToEntity(StadiumRequest request)
    {
        return new Stadium(
            request.Name,
            request.City,
            request.Description,
            request.Capacity
        );
    }

    public static StadiumResponse FromEntity(Stadium stadium)
    {
        return new StadiumResponse
        {
            Name = stadium.Name,
            City = stadium.City,
            Description = stadium.Description,
            Capacity = stadium.Capacity
        };
    }
}