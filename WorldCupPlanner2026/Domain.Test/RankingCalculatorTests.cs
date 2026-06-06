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
    
    [TestMethod]
    public void CalculateDelta_InKnockoutPhase_ShouldApplyOnePointFiveMultiplier()
    {
        var groupDelta = RankingCalculator.CalculateDelta(
            teamRanking: 1500, opponentRanking: 1500, result: 1.0, phase: TournamentPhase.GroupStage);

        var knockoutDelta = RankingCalculator.CalculateDelta(
            teamRanking: 1500, opponentRanking: 1500, result: 1.0, phase: TournamentPhase.QuarterFinals);

        // Mismo escenario, solo cambia la fase → el delta de eliminatorias es 1.5x el de grupos.
        Assert.AreEqual(groupDelta * 1.5, knockoutDelta, 0.0001);
    }
    
    [TestMethod]
    public void CalculateDelta_DrawBetweenEqualRankedTeams_ShouldBeApproximatelyZero()
    {
        var delta = RankingCalculator.CalculateDelta(
            teamRanking: 1500, opponentRanking: 1500, result: 0.5, phase: TournamentPhase.GroupStage);

        // Entre iguales, P = 0.5, resultado 0.5 → delta = 30 * (0.5 - 0.5) = 0
        Assert.AreEqual(0.0, delta, 0.0001);
    }
}