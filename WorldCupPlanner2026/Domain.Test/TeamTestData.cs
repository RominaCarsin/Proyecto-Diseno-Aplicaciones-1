namespace Domain.Test;

public static class TeamTestData
{
    public static Team CreateUruguayTeam() => new("URUGUAY", "CONMEBOL", 1500);
    public static Team CreateArgentinaTeam() => new("ARGENTINA", "UEFA", 1700);
    public static Team CreateBrazilTeam() => new("BRAZIL", "AFC", 1800);
    public static Team CreateChileTeam() => new("CHILE", "CAF", 1400);
}
