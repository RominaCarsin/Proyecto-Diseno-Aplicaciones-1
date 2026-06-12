namespace Domain;

public class GroupPhase
{
    private const int GroupCount = 12;
    public const int RequiredTeamCount = 48;
    public const int MinStadiumCount = 4;
    private const int MaxTeamsPerGroup = 4;
    private const int RoundsPerGroup = 3;
    private const int MatchesPerRound = 2;
    private const int FirstMatchHour = 14;
    private const int HoursBetweenMatches = 4;
    private const int DaysBetweenRounds = 3;
    private const int DaysBetweenGroupStarts = 3;

    private const string NotEnoughTeamsErrorMessage =
        "Se necesitan 48 equipos para generar el fixture.";
    private const string NotEnoughStadiumsErrorMessage =
        "Se necesitan al menos 4 estadios para generar el fixture.";
    private const string TeamNotAssignedErrorMessage =
        "No se pudo asignar el equipo {0} a ningún grupo.";

    private static readonly List<(int homeIndex, int awayIndex)> GroupStagePairings = new()
    {
        (0, 3), (1, 2),
        (0, 2), (1, 3),
        (0, 1), (2, 3)
    };

    private readonly List<Pot> _pots;
    private readonly List<Stadium> _stadiums;
    private readonly DateTime _startDate;
    private readonly List<Group> _groups = new();

    public IReadOnlyCollection<Group> Groups => _groups;

    public GroupPhase(IEnumerable<Pot> pots, IEnumerable<Stadium> stadiums, DateTime startDate)
    {
        _pots = pots?.ToList() ?? new List<Pot>();
        _stadiums = stadiums?.ToList() ?? new List<Stadium>();
        _startDate = startDate;
    }

    public void Generate()
    {
        ValidateTeamCount();
        ValidateStadiumCount();
        CreateGroups();
        AssignTeamsToGroupsFromPots();
        GenerateMatchesForGroups();
    }
    
    private void ValidateTeamCount()
    {
        int totalTeams = _pots.Sum(p => p.Teams.Count);

        if (totalTeams < RequiredTeamCount)
            throw new InvalidOperationException(NotEnoughTeamsErrorMessage);
    }

    private void ValidateStadiumCount()
    {
        if (_stadiums.Count < MinStadiumCount)
            throw new InvalidOperationException(NotEnoughStadiumsErrorMessage);
    }
    
    private void CreateGroups()
    {
        _groups.Clear();

        for (int i = 0; i < GroupCount; i++)
            _groups.Add(new Group((char)('A' + i)));
    }
    
    private void AssignTeamsToGroupsFromPots()
    {
        foreach (var pot in _pots)
        {
            foreach (var team in pot.Teams)
            {
                int preferredGroupIndex = FindGroupIndexWithFewestSameConfederationTeams(team);
                AssignTeamToGroupStartingFrom(team, preferredGroupIndex);
            }
        }
    }

    private int FindGroupIndexWithFewestSameConfederationTeams(Team team) =>
        _groups
            .Select((group, index) => (group, index))
            .Where(x => x.group.Teams.Count < MaxTeamsPerGroup)
            .OrderBy(x => x.group.Teams.Count(t =>
                t.ConfederationName.Equals(team.ConfederationName, StringComparison.OrdinalIgnoreCase)))
            .ThenBy(x => x.group.Teams.Count)
            .Select(x => x.index)
            .FirstOrDefault(0);

    private void AssignTeamToGroupStartingFrom(Team team, int startGroupIndex)
    {
        for (int offset = 0; offset < _groups.Count; offset++)
        {
            var group = _groups[(startGroupIndex + offset) % _groups.Count];

            try
            {
                group.AddTeam(team);
                return;
            }
            catch (ArgumentException)
            {
            }
        }

        throw new InvalidOperationException(
            string.Format(TeamNotAssignedErrorMessage, team.Name));
    }
    
    private void GenerateMatchesForGroups()
    {
        int matchCounter = 0;

        for (int groupIndex = 0; groupIndex < _groups.Count; groupIndex++)
        {
            var group = _groups[groupIndex];
            var groupTeams = group.Teams.ToList();
            int groupStartDay = groupIndex * DaysBetweenGroupStarts;

            for (int round = 0; round < RoundsPerGroup; round++)
            {
                var roundDate = _startDate.Date.AddDays(groupStartDay + round * DaysBetweenRounds);
                var roundPairings = GroupStagePairings
                    .Skip(round * MatchesPerRound)
                    .Take(MatchesPerRound)
                    .ToList();

                for (int matchIndex = 0; matchIndex < roundPairings.Count; matchIndex++)
                {
                    var matchDate = CalculateMatchDate(roundDate, round, matchIndex);
                    AddMatchToGroup(group, groupTeams, roundPairings[matchIndex], matchDate, ref matchCounter);
                }
            }
        }
    }

    private static DateTime CalculateMatchDate(DateTime roundDate, int round, int matchIndex)
    {
        bool isLastRound = round == RoundsPerGroup - 1;

        return isLastRound
            ? roundDate.AddHours(FirstMatchHour)
            : roundDate.AddHours(FirstMatchHour + matchIndex * HoursBetweenMatches);
    }

    private void AddMatchToGroup(
        Group group,
        List<Team> groupTeams,
        (int homeIndex, int awayIndex) pairing,
        DateTime matchDate,
        ref int matchCounter)
    {
        var stadium = _stadiums[matchCounter % _stadiums.Count];

        group.AddMatch(new Match(
            matchDate,
            stadium,
            groupTeams[pairing.homeIndex],
            groupTeams[pairing.awayIndex],
            group: group.Label,
            phase: TournamentPhase.GroupStage
        ));

        matchCounter++;
    }
}