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

    private const string NotEnoughStadiumsErrorMessage = "Se necesitan al menos 4 estadios para generar el fixture.";
    private const string NotEnoughTeamsErrorMessage = "Se necesitan 48 equipos para generar el fixture.";
    private const string TeamNotAssignedErrorMessage = "No se pudo asignar el equipo {0} a ningún grupo.";
    
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
        _groups.Clear();
        CreateGroups();
        AssignTeamsToGroupsFromPots();
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
}