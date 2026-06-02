namespace Domain;

public class Fixture
{
    private readonly List<Pot> _pots;
    private readonly List<Stadium> _stadiums;
    private readonly DateTime _startDate;
    private readonly List<Group> _groups = new();

    public IReadOnlyCollection<Group> Groups => _groups;
    public const int GroupCount = 12;
    public Fixture(IEnumerable<Pot> pots, IEnumerable<Stadium> stadiums, DateTime startDate)
    {
        _pots = pots?.ToList() ?? new List<Pot>();
        _stadiums = stadiums?.ToList() ?? new List<Stadium>();
        _startDate = startDate;
    }
    
    

    public void Generate()
    {
        _groups.Clear();

        for (int i = 0; i < GroupCount; i++)
            _groups.Add(new Group((char)('A' + i)));
    }
}