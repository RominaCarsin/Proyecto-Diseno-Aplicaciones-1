namespace Domain;

public static class SimulationEngineFactory
{
    private const string InvalidSimulationEngineTypeErrorMessage =
        "El tipo de motor de simulación es inválido.";

    public static ISimulationEngine Create(SimulationEngineType type)
    {
        if (type == SimulationEngineType.Probabilistic)
            return new ProbabilisticSimulationEngine();

        if (type == SimulationEngineType.PureRandom)
            return new PureRandomSimulationEngine();

        throw new ArgumentException(InvalidSimulationEngineTypeErrorMessage);
    }
}