namespace Domain.Tests;

internal static class GroupPhaseTestHelpers
{
    internal static readonly DateTime StartDate = new(2026, 6, 1);

    internal static List<Team> BuildTeams(
        string confederation,
        string prefix,
        int count,
        int startRanking)
    {
        var teams = new List<Team>(count);

        for (int i = 0; i < count; i++)
            teams.Add(new Team($"{prefix}{i:00}", confederation, startRanking + i * 7));

        return teams;
    }

    internal static List<Stadium> BuildStadiums(int count)
    {
        var stadiums = new List<Stadium>(count);

        for (int i = 0; i < count; i++)
            stadiums.Add(new Stadium($"STADIUM_{i:00}", "CITY", "Historic stadium", 30000));

        return stadiums;
    }

    internal static List<Pot> BuildValidPots() => new()
    {
        new Pot(1, BuildTeams("UEFA",     "UEA-", count: 12, startRanking:  600)),
        new Pot(2, BuildTeams("UEFA",     "UEB-", count: 12, startRanking:  800)),
        new Pot(3, BuildTeams("CONMEBOL", "CON-", count: 12, startRanking: 1000)),
        new Pot(4, BuildTeams("AFC",      "AFC-", count: 12, startRanking: 1200)),
    };

    internal static List<Pot> BuildPotsWithLessThanRequiredTeams() => new()
    {
        new Pot(1, BuildTeams("UEFA",     "A-", count: 12, startRanking:  600)),
        new Pot(2, BuildTeams("CONMEBOL", "B-", count: 12, startRanking:  800)),
        new Pot(3, BuildTeams("AFC",      "C-", count: 12, startRanking: 1000)),
    };

    internal static List<Pot> BuildPotsWithMoreThanRequiredTeams() => new()
    {
        new Pot(1, BuildTeams("UEFA",     "UEA-", count: 12, startRanking:  600)),
        new Pot(2, BuildTeams("UEFA",     "UEB-", count: 12, startRanking:  800)),
        new Pot(3, BuildTeams("CONMEBOL", "CON-", count: 12, startRanking: 1000)),
        new Pot(4, BuildTeams("AFC",      "AFC-", count: 12, startRanking: 1200)),
        new Pot(4, BuildTeams("CAF",      "CAF-", count: 12, startRanking: 1400)),
    };

    internal static List<Pot> BuildPotsWithOnlyConmebolTeams() => new()
    {
        new Pot(1, BuildTeams("CONMEBOL", "CON-A-", count: 12, startRanking:  600)),
        new Pot(2, BuildTeams("CONMEBOL", "CON-B-", count: 12, startRanking:  800)),
        new Pot(3, BuildTeams("CONMEBOL", "CON-C-", count: 12, startRanking: 1000)),
        new Pot(4, BuildTeams("CONMEBOL", "CON-D-", count: 12, startRanking: 1200)),
    };
}
