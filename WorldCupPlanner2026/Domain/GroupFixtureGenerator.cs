namespace Domain;

public class GroupFixtureGenerator
{
    private static readonly List<(int homeIndex, int awayIndex)> GroupStagePairings = new()
    {
        (0, 3), (1, 2),
        (0, 2), (1, 3),
        (0, 1), (2, 3)
    };

    private readonly IMatchFactory _matchFactory;
    private readonly TournamentRules _rules;

    public GroupFixtureGenerator(IMatchFactory matchFactory, TournamentRules rules)
    {
        _matchFactory = matchFactory;
        _rules = rules;
    }

    public void Generate(List<Group> groups, List<Stadium> stadiums, DateTime startDate)
    {
        int matchCounter = 0;

        for (int groupIndex = 0; groupIndex < groups.Count; groupIndex++)
        {
            var group = groups[groupIndex];
            var groupTeams = group.Teams.ToList();
            int groupStartDay = groupIndex * _rules.DaysBetweenGroupStarts;

            for (int round = 0; round < _rules.RoundsPerGroup; round++)
            {
                var roundDate = startDate.Date.AddDays(groupStartDay + round * _rules.DaysBetweenRounds);
                var roundPairings = GroupStagePairings
                    .Skip(round * _rules.MatchesPerRound)
                    .Take(_rules.MatchesPerRound)
                    .ToList();

                for (int matchIndex = 0; matchIndex < roundPairings.Count; matchIndex++)
                {
                    var matchDate = CalculateMatchDate(roundDate, round, matchIndex);
                    AddMatchToGroup(group, groupTeams, roundPairings[matchIndex], matchDate, stadiums, ref matchCounter);
                }
            }
        }
    }

    private DateTime CalculateMatchDate(DateTime roundDate, int round, int matchIndex)
    {
        bool isLastRound = round == _rules.RoundsPerGroup - 1;

        return isLastRound
            ? roundDate.AddHours(_rules.FirstMatchHour)
            : roundDate.AddHours(_rules.FirstMatchHour + matchIndex * _rules.HoursBetweenMatches);
    }

    private void AddMatchToGroup(
        Group group,
        List<Team> groupTeams,
        (int homeIndex, int awayIndex) pairing,
        DateTime matchDate,
        List<Stadium> stadiums,
        ref int matchCounter)
    {
        var stadium = stadiums[matchCounter % stadiums.Count];
        var match = _matchFactory.CreateGroupMatch(
            matchDate,
            stadium,
            groupTeams[pairing.homeIndex],
            groupTeams[pairing.awayIndex],
            group.Label
        );

        group.AddMatch(match);
        matchCounter++;
    }
}
