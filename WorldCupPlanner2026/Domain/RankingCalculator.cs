namespace Domain;

public class RankingCalculator
{
    private const int DevelopmentFactor = 30;
    private const int MinRanking = 300;
    private const int MaxRanking = 2500;

    public static int CalculateNewRanking(int teamRanking, int opponentRanking, double result, TournamentPhase phase)
    {
        var delta = CalculateDelta(teamRanking, opponentRanking, result, phase);
        var updatedRanking = RoundRanking(teamRanking + delta);

        return Clamp(updatedRanking);
    }

    public static double CalculateDelta(int teamRanking, int opponentRanking, double result, TournamentPhase phase)
    {
        var probability = 1.0 / (1.0 + Math.Pow(10, (opponentRanking - teamRanking) / 1000.0));
        var multiplier = PhaseMultiplier(phase);

        return DevelopmentFactor * (result - probability) * multiplier;
    }

    private static int RoundRanking(double ranking)
    {
        return (int)Math.Round(ranking, MidpointRounding.AwayFromZero);
    }

    private static int Clamp(int ranking)
    {
        if (ranking < MinRanking) return MinRanking;
        if (ranking > MaxRanking) return MaxRanking;
        return ranking;
    }

    private static double PhaseMultiplier(TournamentPhase phase) =>
        phase == TournamentPhase.GroupStage ? 1.0 : 1.5;
}