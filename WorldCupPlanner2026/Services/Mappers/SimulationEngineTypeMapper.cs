using Domain;
using DTOs;

namespace Services.Mappers;

public static class SimulationEngineTypeMapper
{
    private const string InvalidSimulationEngineTypeErrorMessage =
        "El tipo de motor de simulación es inválido.";

    public static SimulationEngineType ToDomain(SimulationEngineTypeDto type) =>
        type switch
        {
            SimulationEngineTypeDto.Probabilistic => SimulationEngineType.Probabilistic,
            SimulationEngineTypeDto.PureRandom => SimulationEngineType.PureRandom,
            _ => throw new ArgumentException(InvalidSimulationEngineTypeErrorMessage)
        };
}