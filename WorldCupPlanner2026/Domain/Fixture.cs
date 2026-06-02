namespace Domain;

public class Fixture
{
    private readonly List<Pot> _pots;
    private readonly List<Stadium> _stadiums;
    private readonly DateTime _startDate;
    private readonly List<Group> _groups = new();

    public IReadOnlyCollection<Group> Groups => _groups;
    public const int GroupCount = 12;
    public const int RequiredTeamCount = 48;
    public const int MinStadiumCount = 4;
    public const int MaxTeamsPerGroup = 4;
    public const int FirstMatchHour = 14;
    public const int HoursBetweenMatches = 4;
    public const int RoundDaysOffset = 3;
    public const int MatchesPerRound = 2;

    private const string NotEnoughStadiumsErrorMessage = "Se necesitan al menos 4 estadios para generar el fixture.";
    private const string NotEnoughTeamsErrorMessage = "Se necesitan 48 equipos para generar el fixture.";
    private const string TeamNotAssignedErrorMessage = "No se pudo asignar el equipo {0} a ningún grupo.";
    private const string FixtureNotGeneratedErrorMessage = "No se ha generado el fixture todavía.";
    
    public Fixture(IEnumerable<Pot> pots, IEnumerable<Stadium> stadiums, DateTime startDate)
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
        var totalTeams = _pots.Sum(p => p.Teams.Count);

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
            var teams = pot.Teams;

            for (int i = 0; i < teams.Count; i++)
            {
                int bestStart = FindBestStartIndex(teams[i]);
                AddTeamToNextAvailableGroup(teams[i], bestStart);
            }
        }
    }
    
    
    private int FindBestStartIndex(Team team) =>
        _groups
            .Select((g, index) => (group: g, index))
            .Where(x => x.group.Teams.Count < MaxTeamsPerGroup)
            .OrderBy(x => x.group.Teams.Count(t => t.ConfederationName == team.ConfederationName))
            .ThenBy(x => x.group.Teams.Count)
            .Select(x => x.index)
            .FirstOrDefault(0);

    private void AddTeamToNextAvailableGroup(Team team, int startGroupIndex)
    {
        int count = _groups.Count;

        for (int i = 0; i < count; i++)
        {
            var group = _groups[(startGroupIndex + i) % count];

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
            var teamList = group.Teams.ToList();

            var pairings = new List<(int, int)>
            {
                (0, 3), (1, 2),
                (0, 2), (1, 3),
                (0, 1), (2, 3)
            };

            int groupStartDayOffset = groupIndex * RoundDaysOffset;

            for (int round = 0; round < 3; round++)
            {
                var roundDate = _startDate.Date.AddDays(groupStartDayOffset + round * RoundDaysOffset);

                var roundPairings = pairings
                    .Skip(round * MatchesPerRound)
                    .Take(MatchesPerRound)
                    .ToList();

                for (int matchIndex = 0; matchIndex < roundPairings.Count; matchIndex++)
                {
                    var matchDate = round == 2
                        ? roundDate.AddHours(FirstMatchHour)
                        : roundDate.AddHours(FirstMatchHour).AddHours(matchIndex * HoursBetweenMatches);

                    AddMatchToGroup(group, teamList, roundPairings[matchIndex], matchDate, ref matchCounter);
                }
            }
        }
    }

    private void AddMatchToGroup(
        Group group,
        List<Team> teamList,
        (int homeIndex, int awayIndex) pairing,
        DateTime matchDate,
        ref int matchCounter)
    {
        var stadium = _stadiums[matchCounter % _stadiums.Count];

        group.AddMatch(new Match(
            matchDate,
            stadium,
            teamList[pairing.homeIndex],
            teamList[pairing.awayIndex],
            group: group.Label,
            phase: TournamentPhase.GroupStage
        ));

        matchCounter++;
    }
    
    public void SimulateGroupStage(int seed)
    {
        ValidateFixtureWasGenerated();

        foreach (var group in _groups)
            group.SimulateMatches(seed);
    }
    
    private void ValidateFixtureWasGenerated()
    {
        if (_groups.Count == 0)
            throw new InvalidOperationException(FixtureNotGeneratedErrorMessage);
    }
    
    private Fixture()
    {
        _pots = new List<Pot>();
        _stadiums = new List<Stadium>();
    }

    public static Fixture FromExistingGroups(IEnumerable<Group> groups)
    {
        var fixture = new Fixture();

        if (groups is not null)
            fixture._groups.AddRange(groups);

        return fixture;
    }
    
}