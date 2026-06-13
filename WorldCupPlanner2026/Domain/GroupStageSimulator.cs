namespace Domain;

public class GroupStageSimulator
{
    private readonly int _seed;
    private readonly ISimulationEngine _simulationEngine;

    public GroupStageSimulator(int seed)
        : this(seed, new ProbabilisticSimulationEngine())
    {
    }

    public GroupStageSimulator(int seed, ISimulationEngine simulationEngine)
    {
        _seed = seed;
        _simulationEngine = simulationEngine;
    }

    public void Simulate(IEnumerable<Group> groups)
    {
        foreach (var group in groups)
            SimulateGroup(group);
    }

    private void SimulateGroup(Group group)
    {
        var random = new Random(_seed + group.Label);

        foreach (var match in group.Matches)
        {
            var result = _simulationEngine.Simulate(
                match.HomeTeam,
                match.AwayTeam,
                match.Phase,
                random);

            group.SetMatchResult(match, result.HomeGoals, result.AwayGoals);
        }
    }
}