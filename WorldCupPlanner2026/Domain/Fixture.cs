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
                var group = _groups[i % GroupCount];
                group.AddTeam(teams[i]);
            }
        }
    }
}