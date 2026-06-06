namespace Domain.Test;

[TestClass]
public class RankingCalculatorTests
{
    private const int WinnerWeakRanking = 1200;
    private const int LoserStrongRanking = 1500;

    [TestMethod]
    public void CalculateDelta_WeakTeamBeatsStrongInGroupStage_ShouldMatchStatementExample()
    {
        // Resultado 1 (victoria) para el equipo débil (1200) contra el fuerte (1500), fase de grupos.
        var delta = RankingCalculator.CalculateDelta(
            teamRanking: WinnerWeakRanking,
            opponentRanking: LoserStrongRanking,
            result: 1.0,
            phase: TournamentPhase.GroupStage);
        Assert.AreEqual(20.02, delta, 0.1);
    }
}