namespace Domain.Test;

[TestClass]
public class MatchTests
{
    private static readonly DateTime ValidDate = new(2026, 6, 1);
    private static readonly DateTime AnotherValidDate = new(2026, 6, 10);

    [TestMethod]
    public void CreateGroupStageMatch_WithValidData_ShouldCreateMatchWithExpectedProperties()
    {
        var match = CreateMatch(group: 'A');

        Assert.AreEqual(ValidDate, match.Date);
        Assert.AreEqual("CENTENARIO", match.Stadium.Name);
        Assert.AreEqual("ARGENTINA", match.HomeTeam.Name);
        Assert.AreEqual("BRAZIL", match.AwayTeam.Name);
        Assert.AreEqual('A', match.Group);
        Assert.AreEqual(TournamentPhase.GroupStage, match.Phase);
        Assert.AreEqual(MatchStatus.NotFinished, match.Status);
        Assert.IsFalse(match.HasResult);
        Assert.IsFalse(match.IsLocked);
    }

    [TestMethod]
    public void CreateKnockoutMatch_WithValidData_ShouldCreateMatchWithoutGroup()
    {
        var match = CreateMatch(group: null, phase: TournamentPhase.RoundOf32);

        Assert.IsNull(match.Group);
        Assert.AreEqual(TournamentPhase.RoundOf32, match.Phase);
    }

    [TestMethod]
    public void CreateKnockoutMatch_WithGroupAssigned_ShouldThrowInvalidOperationException()
    {
        Assert.ThrowsException<InvalidOperationException>(() =>
            CreateMatch(group: 'A', phase: TournamentPhase.RoundOf32));
    }

    [TestMethod]
    public void CreateMatch_WithInvalidData_ShouldThrow()
    {
        Assert.ThrowsException<ArgumentException>(() => CreateMatch(date: DateTime.MinValue));
        Assert.ThrowsException<ArgumentException>(() => new Match(ValidDate, null, BuildTeam("ARGENTINA"), BuildTeam("BRAZIL")));
        Assert.ThrowsException<ArgumentException>(() => new Match(ValidDate, BuildStadium(), null, BuildTeam("BRAZIL")));
        Assert.ThrowsException<ArgumentException>(() => new Match(ValidDate, BuildStadium(), BuildTeam("ARGENTINA"), null));

        var team = BuildTeam("ARGENTINA");
        Assert.ThrowsException<ArgumentException>(() => CreateMatch(homeTeam: team, awayTeam: team));
        Assert.ThrowsException<ArgumentException>(() => CreateMatch(homeTeam: BuildTeam("ARGENTINA"), awayTeam: BuildTeam("ARGENTINA")));
    }

    [TestMethod]
    public void SetResult_WithValidGroupStageResults_ShouldSetWinnerAndTeamStats()
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
    public void SetResult_WithExistingResult_ShouldRevertPreviousTeamStats()
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
    public void SetResult_WithInvalidGoals_ShouldThrow()
    {
        var match = CreateMatch();

        Assert.ThrowsException<ArgumentOutOfRangeException>(() => match.SetResult(-1, 0));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => match.SetResult(0, -1));

        var knockout = CreateMatch(group: null, phase: TournamentPhase.RoundOf32);
        Assert.ThrowsException<InvalidOperationException>(() => knockout.SetResult(1, 1));
    }
    
    [TestMethod]
    public void SetResult_RevertingAwayWin_ShouldUpdateTeamStatsCorrectly()
    {
        var homeTeam = BuildTeam("URUGUAY");
        var awayTeam = BuildTeam("ARGENTINA");
        var match = CreateMatch(homeTeam: homeTeam, awayTeam: awayTeam);

        // 1. Seteamos victoria visitante (0-2)
        match.SetResult(0, 2); 
    
        // 2. Cambiamos a victoria local (1-0). Debe revertir la victoria de Argentina.
        match.SetResult(1, 0);

        Assert.AreEqual(3, homeTeam.Points);
        Assert.AreEqual(0, awayTeam.Points);
        Assert.AreEqual(1, homeTeam.GoalDifference);
        Assert.AreEqual(-1, awayTeam.GoalDifference);
        Assert.AreEqual(homeTeam, match.Winner);
    }

    [TestMethod]
    public void SetResult_RevertingDraw_ShouldUpdateTeamStatsCorrectly()
    {
        var homeTeam = BuildTeam("URUGUAY");
        var awayTeam = BuildTeam("ARGENTINA");
        var match = CreateMatch(homeTeam: homeTeam, awayTeam: awayTeam);

        // 1. Seteamos empate (1-1)
        match.SetResult(1, 1); 
        Assert.AreEqual(1, homeTeam.Points);
        Assert.AreEqual(1, awayTeam.Points);

        // 2. Cambiamos a victoria visitante (0-3). Debe revertir el punto del empate.
        match.SetResult(0, 3);

        Assert.AreEqual(0, homeTeam.Points);
        Assert.AreEqual(3, awayTeam.Points);
        Assert.AreEqual(-3, homeTeam.GoalDifference);
        Assert.AreEqual(3, awayTeam.GoalDifference);
        Assert.AreEqual(awayTeam, match.Winner);
    }

    [TestMethod]
    public void UpdateDateAndStadium_WithUnlockedMatch_ShouldReplaceValues()
    {
        var match = CreateMatch();
        var newStadium = new Stadium("MARACANA", "RIO DE JANEIRO", "Historic stadium", 60000);

        match.UpdateDate(AnotherValidDate);
        match.UpdateStadium(newStadium);

        Assert.AreEqual(AnotherValidDate, match.Date);
        Assert.AreEqual("MARACANA", match.Stadium.Name);
    }

    [TestMethod]
    public void LockedMatch_ShouldRejectEditableChanges()
    {
        var match = CreateMatch();
        match.LockEdition();

        Assert.ThrowsException<InvalidOperationException>(() => match.UpdateDate(AnotherValidDate));
        Assert.ThrowsException<InvalidOperationException>(() => match.UpdateStadium(BuildStadium("MARACANA")));
    }

    [TestMethod]
    public void Finish_ShouldSetStatusAndLockMatch()
    {
        var match = CreateMatch();

        match.Finish();

        Assert.AreEqual(MatchStatus.Finished, match.Status);
        Assert.IsTrue(match.IsLocked);
        Assert.ThrowsException<InvalidOperationException>(() => match.Finish());
        Assert.ThrowsException<InvalidOperationException>(() => match.SetResult(1, 0));
    }
    
    [TestMethod]
    public void RegisterIncidence_YellowCard_ShouldBeCountedForThatTeam()
    {
        var homeTeam = BuildTeam("URUGUAY");
        var awayTeam = BuildTeam("ARGENTINA");
        var match = CreateMatch(homeTeam: homeTeam, awayTeam: awayTeam);

        match.RegisterIncidence(new YellowCard(homeTeam, minute: 30));

        Assert.AreEqual(1, match.YellowCardsOf(homeTeam));
        Assert.AreEqual(0, match.YellowCardsOf(awayTeam));
    }

    private static Match CreateMatch(
        DateTime? date = null,
        Stadium? stadium = null,
        Team? homeTeam = null,
        Team? awayTeam = null,
        char? group = 'A',
        TournamentPhase phase = TournamentPhase.GroupStage) =>
        new(
            date: date ?? ValidDate,
            stadium: stadium ?? BuildStadium(),
            homeTeam: homeTeam ?? BuildTeam("ARGENTINA"),
            awayTeam: awayTeam ?? BuildTeam("BRAZIL"),
            group: group,
            phase: phase
        );
    
    [TestMethod]
    public void RegisterIncidence_RedCard_ShouldBeCountedSeparatelyFromYellow()
    {
        var homeTeam = BuildTeam("URUGUAY");
        var awayTeam = BuildTeam("ARGENTINA");
        var match = CreateMatch(homeTeam: homeTeam, awayTeam: awayTeam);

        match.RegisterIncidence(new YellowCard(homeTeam, minute: 30));
        match.RegisterIncidence(new RedCard(homeTeam, minute: 80));
        match.RegisterIncidence(new RedCard(awayTeam, minute: 85));

        Assert.AreEqual(1, match.YellowCardsOf(homeTeam));
        Assert.AreEqual(1, match.RedCardsOf(homeTeam));
        Assert.AreEqual(1, match.RedCardsOf(awayTeam));
    }

    private static Stadium BuildStadium(string name = "CENTENARIO") =>
        new(name, "MONTEVIDEO", "Historic stadium", 60000);

    private static Team BuildTeam(string name) =>
        new(name, "CONMEBOL", 1800);
}