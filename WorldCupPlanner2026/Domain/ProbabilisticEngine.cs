namespace Domain;

public sealed class ProbabilisticEngine : ISimulationEngine
{
    public string Name => "Probabilístico";
    
    private const double DrawZone = 0.10;

    public (int homeGoals, int awayGoals) SimulateResult(
        int homeRanking, int awayRanking, bool allowDraw, Random random)
    {
        double chance = (double)homeRanking / (homeRanking + awayRanking);
        double roll = random.NextDouble();

        if (!allowDraw)
            return roll < chance ? ScoreLine(true, allowDraw, random)
                : ScoreLine(false, allowDraw, random);

        if (roll < chance - DrawZone) return ScoreLine(true, allowDraw, random);
        if (roll > chance + DrawZone) return ScoreLine(false, allowDraw, random);
        int draw = random.Next(0, 3);
        return (draw, draw);
    }

    private static (int, int) ScoreLine(bool homeWon, bool allowDraw, Random random)
    {
        int winner = random.Next(2, allowDraw ? 5 : 4);
        int loser  = random.Next(0, allowDraw ? 3 : 2);
        return homeWon ? (winner, loser) : (loser, winner);
    }
}