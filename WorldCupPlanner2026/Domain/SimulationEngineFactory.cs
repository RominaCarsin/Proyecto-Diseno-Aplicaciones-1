namespace Domain;

public static class SimulationEngineFactory
{
    private const string InvalidSimulationEngineTypeErrorMessage =
        "El tipo de motor de simulación es inválido.";

    public static ISimulationEngine Create(SimulationEngineType type) => type switch
        {
            SimulationEngineType.Probabilistic => new ProbabilisticSimulationEngine(),
            SimulationEngineType.PureRandom => new PureRandomSimulationEngine(),
            _ => throw new ArgumentException(InvalidSimulationEngineTypeErrorMessage)
        };
}