namespace Domain;

public class RankingCalculator
{
    private const int DevelopmentFactor = 30;

    public static double CalculateDelta(int teamRanking, int opponentRanking, double result, TournamentPhase phase)
    {
        var probability = 1.0 / (1.0 + Math.Pow(10, (opponentRanking - teamRanking) / 1000.0));
        var multiplier = PhaseMultiplier(phase);

        return DevelopmentFactor * (result - probability) * multiplier;
    }

    private static double PhaseMultiplier(TournamentPhase phase) =>
        phase == TournamentPhase.GroupStage ? 1.0 : 1.5;
}