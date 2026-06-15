namespace Domain;

public readonly record struct SimulationResult(
    int HomeGoals,
    int AwayGoals,
    int HomeYellowCards = 0,
    int AwayYellowCards = 0,
    int HomeRedCards = 0,
    int AwayRedCards = 0);
