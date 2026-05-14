namespace Domain.Test;

[TestClass]
public class MatchTests
{
    private static readonly DateTime ValidDate = new(2026, 6, 1);
    private static readonly DateTime AnotherValidDate = new(2026, 6, 10);
    
    [TestMethod]
    public void CreateGroupStageMatch_WithValidData_ShouldCreateMatchWithExpectedProperties()
    {
        var group = new Group('A');

        var match = CreateMatch(group: group);

        Assert.AreEqual(ValidDate, match.Date);
        Assert.AreEqual("CENTENARIO", match.Stadium.Name);
        Assert.AreEqual("ARGENTINA", match.HomeTeam.Name);
        Assert.AreEqual("BRAZIL", match.AwayTeam.Name);
        Assert.AreEqual(group, match.Group);
        Assert.AreEqual(TournamentPhase.GroupStage, match.Phase);
        Assert.AreEqual(MatchStatus.NotFinished, match.Status);
        Assert.IsFalse(match.HasResult);
        Assert.IsFalse(match.IsLocked);
    }

    [TestMethod]
    public void CreateKnockoutMatch_WithValidData_ShouldCreateMatchWithoutGroup()
    {
        var match = CreateMatch(phase: TournamentPhase.RoundOf32);

        Assert.IsNull(match.Group);
        Assert.AreEqual(TournamentPhase.RoundOf32, match.Phase);
    }

    [TestMethod]
    public void CreateKnockoutMatch_WithGroupAssigned_ShouldThrowInvalidOperationException()
    {
        Assert.ThrowsException<InvalidOperationException>(() =>
            CreateMatch(group: new Group('A'), phase: TournamentPhase.RoundOf32));
    }

    [TestMethod]
    public void CreateMatch_WithMinValueDate_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            CreateMatch(date: DateTime.MinValue));
    }

    [TestMethod]
    public void CreateMatch_WithNullStadium_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Match(ValidDate, null, BuildTeam("ARGENTINA"), BuildTeam("BRAZIL")));
    }

    [TestMethod]
    public void CreateMatch_WithNullHomeTeam_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Match(ValidDate, BuildStadium(), null, BuildTeam("BRAZIL")));
    }

    [TestMethod]
    public void CreateMatch_WithNullAwayTeam_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Match(ValidDate, BuildStadium(), BuildTeam("ARGENTINA"), null));
    }

    [TestMethod]
    public void CreateMatch_WithSameTeamReferenceForBothRoles_ShouldThrowArgumentException()
    {
        var sameTeam = BuildTeam("ARGENTINA");

        Assert.ThrowsException<ArgumentException>(() =>
            CreateMatch(homeTeam: sameTeam, awayTeam: sameTeam));
    }

    [TestMethod]
    public void CreateMatch_WithDifferentInstancesOfSameTeamName_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            CreateMatch(homeTeam: BuildTeam("ARGENTINA"), awayTeam: BuildTeam("ARGENTINA")));
    }
    
    [TestMethod]
    public void SetResult_WithHomeTeamScoringMore_ShouldSetHomeTeamAsWinnerAndUpdateStats()
    {
        var homeTeam = BuildTeam("URUGUAY");
        var awayTeam = BuildTeam("ARGENTINA");
        var match = CreateMatch(homeTeam: homeTeam, awayTeam: awayTeam);

        match.SetResult(3, 1);

        Assert.AreEqual(3, match.HomeGoals);
        Assert.AreEqual(1, match.AwayGoals);
        Assert.AreEqual(homeTeam, match.Winner);
        Assert.AreEqual(3, homeTeam.Points);
        Assert.AreEqual(0, awayTeam.Points);
        Assert.AreEqual(2, homeTeam.GoalDifference);
        Assert.AreEqual(-2, awayTeam.GoalDifference);
    }

    [TestMethod]
    public void SetResult_WithExistingHomeWin_WhenReplacedWithAwayWin_ShouldRevertAndApplyNewResult()
    {
        var homeTeam = BuildTeam("URUGUAY");
        var awayTeam = BuildTeam("ARGENTINA");
        var match = CreateMatch(homeTeam: homeTeam, awayTeam: awayTeam);

        match.SetResult(2, 0);
        match.SetResult(0, 1);

        Assert.AreEqual(0, homeTeam.Points);
        Assert.AreEqual(3, awayTeam.Points);
        Assert.AreEqual(-1, homeTeam.GoalDifference);
        Assert.AreEqual(1, awayTeam.GoalDifference);
        Assert.AreEqual(0, homeTeam.GoalsInFavor);
        Assert.AreEqual(1, awayTeam.GoalsInFavor);
        Assert.AreEqual(awayTeam, match.Winner);
    }

    [TestMethod]
    public void SetResult_WithExistingHomeWin_WhenReplacedWithDraw_ShouldRevertAndApplyDraw()
    {
        var homeTeam = BuildTeam("URUGUAY");
        var awayTeam = BuildTeam("ARGENTINA");
        var match = CreateMatch(homeTeam: homeTeam, awayTeam: awayTeam);

        match.SetResult(2, 0);
        match.SetResult(1, 1);

        Assert.AreEqual(1, homeTeam.Points);
        Assert.AreEqual(1, awayTeam.Points);
        Assert.AreEqual(0, homeTeam.GoalDifference);
        Assert.AreEqual(0, awayTeam.GoalDifference);
        Assert.AreEqual(1, homeTeam.GoalsInFavor);
        Assert.AreEqual(1, awayTeam.GoalsInFavor);
        Assert.IsNull(match.Winner);
    }

    [TestMethod]
    public void SetResult_WithExistingAwayWin_WhenReplacedWithHomeWin_ShouldRevertAndApplyNewResult()
    {
        var homeTeam = BuildTeam("URUGUAY");
        var awayTeam = BuildTeam("ARGENTINA");
        var match = CreateMatch(homeTeam: homeTeam, awayTeam: awayTeam);

        match.SetResult(0, 2);
        match.SetResult(3, 0);

        Assert.AreEqual(3, homeTeam.Points);
        Assert.AreEqual(0, awayTeam.Points);
        Assert.AreEqual(3, homeTeam.GoalDifference);
        Assert.AreEqual(-3, awayTeam.GoalDifference);
        Assert.AreEqual(3, homeTeam.GoalsInFavor);
        Assert.AreEqual(0, awayTeam.GoalsInFavor);
        Assert.AreEqual(homeTeam, match.Winner);
    }

    [TestMethod]
    public void SetResult_WithExistingDraw_WhenReplacedWithHomeWin_ShouldRevertAndApplyNewResult()
    {
        var homeTeam = BuildTeam("URUGUAY");
        var awayTeam = BuildTeam("ARGENTINA");
        var match = CreateMatch(homeTeam: homeTeam, awayTeam: awayTeam);

        match.SetResult(1, 1);
        match.SetResult(2, 0);

        Assert.AreEqual(3, homeTeam.Points);
        Assert.AreEqual(0, awayTeam.Points);
        Assert.AreEqual(2, homeTeam.GoalDifference);
        Assert.AreEqual(-2, awayTeam.GoalDifference);
        Assert.AreEqual(2, homeTeam.GoalsInFavor);
        Assert.AreEqual(0, awayTeam.GoalsInFavor);
        Assert.AreEqual(homeTeam, match.Winner);
    }

    [TestMethod]
    public void SetResult_WithNegativeHomeGoals_ShouldThrowArgumentOutOfRangeException()
    {
        var match = CreateMatch();

        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            match.SetResult(-1, 0));
    }

    [TestMethod]
    public void SetResult_WithNegativeAwayGoals_ShouldThrowArgumentOutOfRangeException()
    {
        var match = CreateMatch();

        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            match.SetResult(0, -1));
    }

    [TestMethod]
    public void SetResult_WithDrawInKnockoutMatch_ShouldThrowInvalidOperationException()
    {
        var knockoutMatch = CreateMatch(phase: TournamentPhase.RoundOf32);

        Assert.ThrowsException<InvalidOperationException>(() =>
            knockoutMatch.SetResult(1, 1));
    }

    [TestMethod]
    public void SetResult_WhenMatchIsLocked_ShouldThrowInvalidOperationException()
    {
        var match = CreateMatch();
        match.LockEdition();

        Assert.ThrowsException<InvalidOperationException>(() =>
            match.SetResult(1, 0));
    }

    [TestMethod]
    public void SetResult_WhenMatchIsFinished_ShouldThrowInvalidOperationException()
    {
        var match = CreateMatch();
        match.Finish();

        Assert.ThrowsException<InvalidOperationException>(() =>
            match.SetResult(1, 0));
    }
    
    [TestMethod]
    public void UpdateDate_WithValidDate_ShouldReplaceMatchDate()
    {
        var match = CreateMatch();

        match.UpdateDate(AnotherValidDate);

        Assert.AreEqual(AnotherValidDate, match.Date);
    }

    [TestMethod]
    public void UpdateDate_WhenMatchIsLocked_ShouldThrowInvalidOperationException()
    {
        var match = CreateMatch();
        match.LockEdition();

        Assert.ThrowsException<InvalidOperationException>(() =>
            match.UpdateDate(AnotherValidDate));
    }

    [TestMethod]
    public void UpdateDate_WhenMatchIsFinished_ShouldThrowInvalidOperationException()
    {
        var match = CreateMatch();
        match.Finish();

        Assert.ThrowsException<InvalidOperationException>(() =>
            match.UpdateDate(AnotherValidDate));
    }
    
    [TestMethod]
    public void UpdateStadium_WithValidStadium_ShouldReplaceMatchStadium()
    {
        var match = CreateMatch();
        var newStadium = BuildStadium("MARACANA");

        match.UpdateStadium(newStadium);

        Assert.AreEqual("MARACANA", match.Stadium.Name);
    }

    [TestMethod]
    public void UpdateStadium_WhenMatchIsLocked_ShouldThrowInvalidOperationException()
    {
        var match = CreateMatch();
        match.LockEdition();

        Assert.ThrowsException<InvalidOperationException>(() =>
            match.UpdateStadium(BuildStadium("MARACANA")));
    }

    [TestMethod]
    public void UpdateStadium_WhenMatchIsFinished_ShouldThrowInvalidOperationException()
    {
        var match = CreateMatch();
        match.Finish();

        Assert.ThrowsException<InvalidOperationException>(() =>
            match.UpdateStadium(BuildStadium("MARACANA")));
    }
    
    [TestMethod]
    public void Finish_ShouldSetStatusToFinishedAndLockMatch()
    {
        var match = CreateMatch();

        match.Finish();

        Assert.AreEqual(MatchStatus.Finished, match.Status);
        Assert.IsTrue(match.IsLocked);
    }

    [TestMethod]
    public void Finish_WhenMatchIsAlreadyFinished_ShouldThrowInvalidOperationException()
    {
        var match = CreateMatch();
        match.Finish();

        Assert.ThrowsException<InvalidOperationException>(() =>
            match.Finish());
    }
    
    [TestMethod]
    public void LockEdition_ShouldSetIsLockedToTrue()
    {
        var match = CreateMatch();

        match.LockEdition();

        Assert.IsTrue(match.IsLocked);
    }
    
    private static Match CreateMatch(
        DateTime? date = null,
        Stadium? stadium = null,
        Team? homeTeam = null,
        Team? awayTeam = null,
        Group? group = null,
        TournamentPhase phase = TournamentPhase.GroupStage) =>
        new(
            date: date ?? ValidDate,
            stadium: stadium ?? BuildStadium(),
            homeTeam: homeTeam ?? BuildTeam("ARGENTINA"),
            awayTeam: awayTeam ?? BuildTeam("BRAZIL"),
            group: group,
            phase: phase
        );

    private static Stadium BuildStadium(string name = "CENTENARIO") =>
        new(name, "MONTEVIDEO", "Historic stadium", 60000);

    private static Team BuildTeam(string name) =>
        new(name, "CONMEBOL", 1800);
}