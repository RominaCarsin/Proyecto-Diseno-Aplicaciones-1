namespace Domain.Test;

[TestClass]
public class RankingCalculatorTests
{
    private const int WinnerWeakRanking = 1200;
    private const int LoserStrongRanking = 1500;

    [TestMethod]
    public void CalculateDelta_WeakTeamBeatsStrongInGroupStage_ShouldMatchStatementExample()
    {
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
            teamRanking: 1500,
            opponentRanking: 1500,
            result: 1.0,
            phase: TournamentPhase.GroupStage);

        var knockoutDelta = RankingCalculator.CalculateDelta(
            teamRanking: 1500,
            opponentRanking: 1500,
            result: 1.0,
            phase: TournamentPhase.QuarterFinals);

        Assert.AreEqual(groupDelta * 1.5, knockoutDelta, 0.0001);
    }

    [TestMethod]
    public void CalculateDelta_DrawBetweenEqualRankedTeams_ShouldBeApproximatelyZero()
    {
        var delta = RankingCalculator.CalculateDelta(
            teamRanking: 1500,
            opponentRanking: 1500,
            result: 0.5,
            phase: TournamentPhase.GroupStage);

        Assert.AreEqual(0.0, delta, 0.0001);
    }

    [TestMethod]
    public void CalculateNewRanking_WeakTeamBeatsStrongInGroupStage_ShouldReturnRankingPlusRoundedDelta()
    {
        var newRanking = RankingCalculator.CalculateNewRanking(
            teamRanking: 1200,
            opponentRanking: 1500,
            result: 1.0,
            phase: TournamentPhase.GroupStage);

        Assert.AreEqual(1220, newRanking);
    }

    [TestMethod]
    public void CalculateNewRanking_StrongTeamLosesAgainstWeakInGroupStage_ShouldReturnRankingMinusRoundedDelta()
    {
        var newRanking = RankingCalculator.CalculateNewRanking(
            teamRanking: 1500,
            opponentRanking: 1200,
            result: 0.0,
            phase: TournamentPhase.GroupStage);

        Assert.AreEqual(1480, newRanking);
    }

    [TestMethod]
    public void CalculateNewRanking_DrawBetweenEqualRankedTeams_ShouldReturnSameRanking()
    {
        var newRanking = RankingCalculator.CalculateNewRanking(
            teamRanking: 1500,
            opponentRanking: 1500,
            result: 0.5,
            phase: TournamentPhase.GroupStage);

        Assert.AreEqual(1500, newRanking);
    }

    [TestMethod]
    public void CalculateNewRanking_InKnockoutPhase_ShouldApplyOnePointFiveMultiplierToFinalRanking()
    {
        var newRanking = RankingCalculator.CalculateNewRanking(
            teamRanking: 1500,
            opponentRanking: 1500,
            result: 1.0,
            phase: TournamentPhase.QuarterFinals);

        Assert.AreEqual(1523, newRanking);
    }

    [TestMethod]
    public void CalculateDelta_StrongTeamLosesToWeakInGroupStage_ShouldReturnNegativeDelta()
    {
        var delta = RankingCalculator.CalculateDelta(
            teamRanking: 1500,
            opponentRanking: 1200,
            result: 0.0,
            phase: TournamentPhase.GroupStage);

        Assert.IsTrue(delta < 0);
    }

    [TestMethod]
    public void CalculateNewRanking_WhenRankingEndsInHalfPointAfterKnockoutLoss_ShouldRoundAwayFromZero()
    {
        var newRanking = RankingCalculator.CalculateNewRanking(
            teamRanking: 1500,
            opponentRanking: 1500,
            result: 0.0,
            phase: TournamentPhase.QuarterFinals);

        Assert.AreEqual(1478, newRanking);
    }

    [TestMethod]
    public void CalculateNewRanking_WhenCalculatedRankingIsBelowMinimum_ShouldReturnMinimumRanking()
    {
        var newRanking = RankingCalculator.CalculateNewRanking(
            teamRanking: 300,
            opponentRanking: 2000,
            result: 0.0,
            phase: TournamentPhase.QuarterFinals);

        Assert.AreEqual(300, newRanking);
    }

    [TestMethod]
    public void CalculateNewRanking_WhenCalculatedRankingIsAboveMaximum_ShouldReturnMaximumRanking()
    {
        var newRanking = RankingCalculator.CalculateNewRanking(
            teamRanking: 2500,
            opponentRanking: 2500,
            result: 1.0,
            phase: TournamentPhase.QuarterFinals);

        Assert.AreEqual(2500, newRanking);
    }
}