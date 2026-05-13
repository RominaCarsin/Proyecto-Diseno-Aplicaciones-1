namespace Domain.Test;

[TestClass]
public class MatchTests
{
    private static readonly DateTime ValidGroupStageDate = new(2026, 6, 1);
    private static readonly DateTime ValidKnockoutDate = new(2026, 6, 28);
    private static readonly DateTime AnotherValidDate = new(2026, 6, 10);

    private const int ValidHomeGoals = 3;
    private const int ValidAwayGoals = 1;

    // ─── Creation ──────────────────────────────────────────────────────────────

    [TestMethod]
    public void CreateGroupStageMatch_WithValidData_ShouldCreateMatchWithExpectedProperties()
    {
        var match = CreateGroupStageMatch();

        Assert.AreEqual(ValidGroupStageDate, match.Date);
        Assert.AreEqual("CENTENARIO", match.Stadium.Name);
        Assert.AreEqual("ARGENTINA", match.HomeTeam.Name);
        Assert.AreEqual("BRAZIL", match.AwayTeam.Name);
        Assert.IsNotNull(match.Group);
        Assert.AreEqual(TournamentPhase.GroupStage, match.Phase);
    }

    [TestMethod]
    public void CreateKnockoutMatch_WithValidData_ShouldCreateMatchWithoutGroup()
    {
        var match = CreateKnockoutMatch();

        Assert.IsNotNull(match);
        Assert.IsNull(match.Group);
        Assert.AreEqual(TournamentPhase.RoundOf32, match.Phase);
    }

    [TestMethod]
    public void CreateMatch_ShouldInitializeResultAndLockValues()
    {
        var match = CreateGroupStageMatch();

        Assert.IsNull(match.HomeGoals);
        Assert.IsNull(match.AwayGoals);
        Assert.IsNull(match.Winner);
        Assert.IsFalse(match.IsLocked);
    }

    [TestMethod]
    public void CreateMatch_ShouldGenerateIncrementalId()
    {
        var firstMatch = CreateGroupStageMatch();

        var secondMatch = CreateGroupStageMatch(
            date: new DateTime(2026, 6, 2),
            homeTeam: BuildTeam(name: "URUGUAY"),
            awayTeam: BuildTeam(name: "CHILE"),
            group: CreateGroupB()
        );

        Assert.AreEqual(firstMatch.Id + 1, secondMatch.Id);
    }

    [TestMethod]
    public void CreateMatch_WithInvalidDate_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            CreateGroupStageMatch(date: DateTime.MinValue));
    }

    [TestMethod]
    public void CreateMatch_WithNullStadium_ShouldThrowArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            new Match(
                date: ValidGroupStageDate,
                stadium: null,
                homeTeam: CreateValidHomeTeam(),
                awayTeam: CreateValidAwayTeam(),
                group: CreateGroupA(),
                phase: TournamentPhase.GroupStage
            ));
    }

    [TestMethod]
    public void CreateMatch_WithNullHomeTeam_ShouldThrowArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            new Match(
                date: ValidGroupStageDate,
                stadium: CreateValidStadium(),
                homeTeam: null,
                awayTeam: CreateValidAwayTeam(),
                group: CreateGroupA(),
                phase: TournamentPhase.GroupStage
            ));
    }

    [TestMethod]
    public void CreateMatch_WithNullAwayTeam_ShouldThrowArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            new Match(
                date: ValidGroupStageDate,
                stadium: CreateValidStadium(),
                homeTeam: CreateValidHomeTeam(),
                awayTeam: null,
                group: CreateGroupA(),
                phase: TournamentPhase.GroupStage
            ));
    }

    [TestMethod]
    public void CreateMatch_WithSameHomeAndAwayTeamReference_ShouldThrowArgumentException()
    {
        var team = CreateValidHomeTeam();

        Assert.ThrowsException<ArgumentException>(() =>
            CreateGroupStageMatch(homeTeam: team, awayTeam: team));
    }

    [TestMethod]
    public void CreateMatch_WithDifferentInstancesButSameTeamName_ShouldThrowArgumentException()
    {
        var homeTeam = new Team("ARGENTINA", "CONMEBOL", 1800);
        var awayTeam = new Team("ARGENTINA", "CONMEBOL", 1750);

        Assert.ThrowsException<ArgumentException>(() =>
            CreateGroupStageMatch(homeTeam: homeTeam, awayTeam: awayTeam));
    }

    [TestMethod]
    public void CreateGroupStageMatch_WithoutGroup_ShouldThrowInvalidOperationException()
    {
        Assert.ThrowsException<InvalidOperationException>(() =>
            new Match(
                date: ValidGroupStageDate,
                stadium: CreateValidStadium(),
                homeTeam: CreateValidHomeTeam(),
                awayTeam: CreateValidAwayTeam(),
                group: null,
                phase: TournamentPhase.GroupStage
            ));
    }

    [TestMethod]
    public void CreateKnockoutMatch_WithGroupAssigned_ShouldThrowInvalidOperationException()
    {
        Assert.ThrowsException<InvalidOperationException>(() =>
            CreateKnockoutMatch(group: CreateGroupA()));
    }

    // ─── SetResult ─────────────────────────────────────────────────────────────

    [TestMethod]
    public void SetResult_WithHomeTeamScoringMore_ShouldSetHomeTeamAsWinner()
    {
        var match = CreateGroupStageMatch();
        int homeGoals = 3, awayGoals = 1;

        match.SetResult(homeGoals, awayGoals);

        Assert.AreEqual(homeGoals, match.HomeGoals);
        Assert.AreEqual(awayGoals, match.AwayGoals);
        Assert.AreEqual(match.HomeTeam, match.Winner);
    }

    [TestMethod]
    public void SetResult_WithAwayTeamScoringMore_ShouldSetAwayTeamAsWinner()
    {
        var match = CreateGroupStageMatch();
        int homeGoals = 1, awayGoals = 2;

        match.SetResult(homeGoals, awayGoals);

        Assert.AreEqual(homeGoals, match.HomeGoals);
        Assert.AreEqual(awayGoals, match.AwayGoals);
        Assert.AreEqual(match.AwayTeam, match.Winner);
    }

    [TestMethod]
    public void SetResult_WithEqualGoalsInGroupStage_ShouldSetWinnerAsNull()
    {
        var match = CreateGroupStageMatch();
        int homeGoals = 1, awayGoals = 1;

        match.SetResult(homeGoals, awayGoals);

        Assert.AreEqual(homeGoals, match.HomeGoals);
        Assert.AreEqual(awayGoals, match.AwayGoals);
        Assert.IsNull(match.Winner);
    }

    [TestMethod]
    public void SetResult_WithNegativeHomeGoals_ShouldThrowArgumentOutOfRangeException()
    {
        var match = CreateGroupStageMatch();

        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            match.SetResult(-1, 2));
    }

    [TestMethod]
    public void SetResult_WithNegativeAwayGoals_ShouldThrowArgumentOutOfRangeException()
    {
        var match = CreateGroupStageMatch();

        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            match.SetResult(2, -1));
    }

    [TestMethod]
    public void SetResult_WithDrawInKnockoutMatch_ShouldThrowInvalidOperationException()
    {
        var match = CreateKnockoutMatch();

        Assert.ThrowsException<InvalidOperationException>(() =>
            match.SetResult(1, 1));
    }

    [TestMethod]
    public void SetResult_WhenMatchIsLocked_ShouldThrowInvalidOperationException()
    {
        var match = CreateLockedMatch();

        Assert.ThrowsException<InvalidOperationException>(() =>
            match.SetResult(ValidHomeGoals, ValidAwayGoals));
    }

    [TestMethod]
    public void SetResult_WhenHomeTeamWins_ShouldUpdateTeamsStats()
    {
        Team homeTeam = BuildTeam(name: "URUGUAY");
        Team awayTeam = BuildTeam(name: "ARGENTINA");

        Match match = CreateGroupStageMatch(
            homeTeam: homeTeam,
            awayTeam: awayTeam);

        match.SetResult(3, 1);

        Assert.AreEqual(3, homeTeam.Points);
        Assert.AreEqual(0, awayTeam.Points);

        Assert.AreEqual(2, homeTeam.GoalDifference);
        Assert.AreEqual(-2, awayTeam.GoalDifference);

        Assert.AreEqual(3, homeTeam.GoalsInFavor);
        Assert.AreEqual(1, awayTeam.GoalsInFavor);
    }

    // ─── UpdateDate ────────────────────────────────────────────────────────────

    [TestMethod]
    public void UpdateDate_WithValidDate_ShouldReplaceMatchDate()
    {
        var match = CreateGroupStageMatch();

        match.UpdateDate(AnotherValidDate);

        Assert.AreEqual(AnotherValidDate, match.Date);
    }

    [TestMethod]
    public void UpdateDate_WithInvalidDate_ShouldThrowArgumentException()
    {
        var match = CreateGroupStageMatch();

        Assert.ThrowsException<ArgumentException>(() =>
            match.UpdateDate(DateTime.MinValue));
    }

    [TestMethod]
    public void UpdateDate_WhenMatchIsLocked_ShouldThrowInvalidOperationException()
    {
        var lockedMatch = CreateLockedMatch();

        Assert.ThrowsException<InvalidOperationException>(() =>
            lockedMatch.UpdateDate(AnotherValidDate));
    }

    // ─── UpdateStadium ─────────────────────────────────────────────────────────

    [TestMethod]
    public void UpdateStadium_WithValidStadium_ShouldReplaceMatchStadium()
    {
        var match = CreateGroupStageMatch();
        var newStadium = BuildStadium(name: "MARACANA", city: "RIO DE JANEIRO");

        match.UpdateStadium(newStadium);

        Assert.AreEqual("MARACANA", match.Stadium.Name);
        Assert.AreEqual("RIO DE JANEIRO", match.Stadium.City);
    }

    [TestMethod]
    public void UpdateStadium_WithNullStadium_ShouldThrowArgumentNullException()
    {
        var match = CreateGroupStageMatch();

        Assert.ThrowsException<ArgumentNullException>(() =>
            match.UpdateStadium(null));
    }

    [TestMethod]
    public void UpdateStadium_WhenMatchIsLocked_ShouldThrowInvalidOperationException()
    {
        var lockedMatch = CreateLockedMatch();
        var newStadium = BuildStadium(name: "MARACANA", city: "RIO DE JANEIRO");

        Assert.ThrowsException<InvalidOperationException>(() =>
            lockedMatch.UpdateStadium(newStadium));
    }

    // ─── LockEdition ────────────────────────────────────────────────────────────

    [TestMethod]
    public void LockEdition_ShouldSetIsLockedToTrue()
    {
        var match = CreateGroupStageMatch();

        match.LockEdition();

        Assert.IsTrue(match.IsLocked);
    }

    // ─── Helpers ───────────────────────────────────────────────────────────────

    private static Match CreateGroupStageMatch(
        DateTime? date = null,
        Stadium? stadium = null,
        Team? homeTeam = null,
        Team? awayTeam = null,
        Group? group = null) =>
        new Match(
            date: date ?? ValidGroupStageDate,
            stadium: stadium ?? CreateValidStadium(),
            homeTeam: homeTeam ?? CreateValidHomeTeam(),
            awayTeam: awayTeam ?? CreateValidAwayTeam(),
            group: group ?? CreateGroupA(),
            phase: TournamentPhase.GroupStage
        );

    private static Match CreateKnockoutMatch(
        DateTime? date = null,
        Stadium? stadium = null,
        Team? homeTeam = null,
        Team? awayTeam = null,
        Group? group = null) =>
        new Match(
            date: date ?? ValidKnockoutDate,
            stadium: stadium ?? CreateValidStadium(),
            homeTeam: homeTeam ?? CreateValidHomeTeam(),
            awayTeam: awayTeam ?? CreateValidAwayTeam(),
            group: group,
            phase: TournamentPhase.RoundOf32
        );

    private static Match CreateLockedMatch()
    {
        var match = CreateGroupStageMatch();
        match.LockEdition();
        return match;
    }

    private static Stadium CreateValidStadium() =>
        BuildStadium(name: "CENTENARIO", city: "MONTEVIDEO");

    private static Team CreateValidHomeTeam() =>
        BuildTeam(name: "ARGENTINA");

    private static Team CreateValidAwayTeam() =>
        BuildTeam(name: "BRAZIL");

    private static Group CreateGroupA() => new('A');
    private static Group CreateGroupB() => new('B');

    private static Stadium BuildStadium(string name, string city = "MONTEVIDEO") =>
        new Stadium(name, city, "Historic stadium", 60000);

    private static Team BuildTeam(string name) =>
        new Team(name, "CONMEBOL", 1800);
}
