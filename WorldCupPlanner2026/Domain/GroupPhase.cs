namespace Domain;

public class GroupPhase
{
    public const int RequiredTeamCount = 48;
    public const int MinStadiumCount = 4;

    private const string NotEnoughTeamsErrorMessage =
        "Se necesitan 48 equipos para generar el fixture.";
    private const string NotEnoughStadiumsErrorMessage =
        "Se necesitan al menos 4 estadios para generar el fixture.";

    private readonly List<Pot> _pots;
    private readonly List<Stadium> _stadiums;
    private readonly DateTime _startDate;
    private readonly List<Group> _groups = new();
    private readonly GroupDrawRules _drawRules;
    private readonly GroupFixtureGenerator _fixtureGenerator;
    private readonly TournamentRules _rules;

    public IReadOnlyCollection<Group> Groups => _groups;

    public GroupPhase(
        IEnumerable<Pot> pots,
        IEnumerable<Stadium> stadiums,
        DateTime startDate,
        IGroupAssignmentStrategy? strategy = null,
        IMatchFactory? matchFactory = null,
        TournamentRules? rules = null)
    {
        _pots = pots?.ToList() ?? new List<Pot>();
        _stadiums = stadiums?.ToList() ?? new List<Stadium>();
        _startDate = startDate;
        _rules = rules ?? new TournamentRules();
        _drawRules = new GroupDrawRules(
            strategy ?? new ConfederationBalancedStrategy(),
            _rules);
        _fixtureGenerator = new GroupFixtureGenerator(
            matchFactory ?? new MatchFactory(),
            _rules);
    }

    public void Generate()
    {
        ValidateTeamCount();
        ValidateStadiumCount();
        CreateGroups();
        _drawRules.AssignTeams(_groups, _pots);
        _fixtureGenerator.Generate(_groups, _stadiums, _startDate);
    }

    private void ValidateTeamCount()
    {
        int totalTeams = _pots.Sum(p => p.Teams.Count);

        if (totalTeams < _rules.RequiredTeamCount)
            throw new InvalidOperationException(NotEnoughTeamsErrorMessage);
    }

    private void ValidateStadiumCount()
    {
        if (_stadiums.Count < _rules.MinStadiumCount)
            throw new InvalidOperationException(NotEnoughStadiumsErrorMessage);
    }

    private void CreateGroups()
    {
        _groups.Clear();

        for (int i = 0; i < _rules.GroupCount; i++)
            _groups.Add(new Group((char)('A' + i)));
    }
}
