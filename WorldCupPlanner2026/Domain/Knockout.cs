namespace Domain;

public class Knockout
{
    public int SimulationSeed { get; private set; }

    public Knockout(int simulationSeed)
    {
        SimulationSeed = simulationSeed;
    }

    public void UpdateSimulationSeed(int newSeed)
    {
        SimulationSeed = newSeed;
    }
}