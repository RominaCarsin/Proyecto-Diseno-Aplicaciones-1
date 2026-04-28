using Domain;

namespace DTOs;

public class ConfederationResponse
{
    public string Name { get; set; } = "";
    public int Slots { get; set; }
    public int RemainingSlots { get; set; }

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