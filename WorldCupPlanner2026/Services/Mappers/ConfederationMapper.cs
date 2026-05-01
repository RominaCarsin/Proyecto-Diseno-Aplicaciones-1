using Domain;
using DTOs;

namespace Services.Mappers;

public static class ConfederationMapper
{
    public static ConfederationResponse FromEntity(Confederation confederation)
    {
        return new ConfederationResponse
        {
            Name           = confederation.Name,
            Slots          = confederation.Slots,
            RemainingSlots = confederation.RemainingSlots
        };
    }
}