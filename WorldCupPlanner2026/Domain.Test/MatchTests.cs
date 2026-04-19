namespace Domain.Test;

using Domain.Exceptions;

[TestClass]
public class MatchTests
{
    private Stadium CreateValidStadium() =>
        new Stadium("Centenario", "Montevideo", "Historic stadium", 60000);

    private Team CreateValidHomeTeam() =>
        new Team("Argentina", Confederation.CONMEBOL, 1800);

    private Team CreateValidAwayTeam() =>
        new Team("Brazil", Confederation.CONMEBOL, 1750);

    private Match CreateValidGroupStageMatch() =>
        new Match(
            date: new DateTime(2026, 6, 1),
            stadium: CreateValidStadium(),
            homeTeam: CreateValidHomeTeam(),
            awayTeam: CreateValidAwayTeam(),
            group: "A",
            phase: TournamentPhase.GroupStage
        );

    private Match CreateValidKnockoutMatch() =>
        new Match(
            date: new DateTime(2026, 7, 1),
            stadium: CreateValidStadium(),
            homeTeam: CreateValidHomeTeam(),
            awayTeam: CreateValidAwayTeam(),
            group: null,
            phase: TournamentPhase.RoundOf32
        );

    [TestMethod]
    public void CreateMatch_WithValidGroupStageData_ShouldSucceed()
    {
        var match = CreateValidGroupStageMatch();

        Assert.IsNotNull(match);
        Assert.AreEqual("A", match.Group);
        Assert.AreEqual(TournamentPhase.GroupStage, match.Phase);
    }

    [TestMethod]
    public void CreateMatch_WithValidKnockoutData_ShouldSucceed()
    {
        var match = CreateValidKnockoutMatch();

        Assert.IsNotNull(match);
        Assert.IsNull(match.Group);
        Assert.AreEqual(TournamentPhase.RoundOf32, match.Phase);
    }

    [TestMethod]
    public void CreateMatch_WithGroupL_ShouldSucceed()
    {
        var match = new Match(
            date: new DateTime(2026, 6, 1),
            stadium: CreateValidStadium(),
            homeTeam: CreateValidHomeTeam(),
            awayTeam: CreateValidAwayTeam(),
            group: "L",
            phase: TournamentPhase.GroupStage
        );

        Assert.AreEqual("L", match.Group);
    }

    [TestMethod]
    public void CreateMatch_WithInvalidDate_ShouldThrowDomainException()
    {
        Assert.ThrowsException<DomainException>(() =>
            new Match(
                date: DateTime.MinValue,
                stadium: CreateValidStadium(),
                homeTeam: CreateValidHomeTeam(),
                awayTeam: CreateValidAwayTeam(),
                group: "A",
                phase: TournamentPhase.GroupStage
            ));
    }

    [TestMethod]
    public void CreateMatch_WithNullStadium_ShouldThrowDomainException()
    {
        Assert.ThrowsException<DomainException>(() =>
            new Match(
                date: new DateTime(2026, 6, 1),
                stadium: null,
                homeTeam: CreateValidHomeTeam(),
                awayTeam: CreateValidAwayTeam(),
                group: "A",
                phase: TournamentPhase.GroupStage
            ));
    }

    [TestMethod]
    public void CreateMatch_WithNullHomeTeam_ShouldThrowDomainException()
    {
        Assert.ThrowsException<DomainException>(() =>
            new Match(
                date: new DateTime(2026, 6, 1),
                stadium: CreateValidStadium(),
                homeTeam: null,
                awayTeam: CreateValidAwayTeam(),
                group: "A",
                phase: TournamentPhase.GroupStage
            ));
    }

    [TestMethod]
    public void CreateMatch_WithNullAwayTeam_ShouldThrowDomainException()
    {
        Assert.ThrowsException<DomainException>(() =>
            new Match(
                date: new DateTime(2026, 6, 1),
                stadium: CreateValidStadium(),
                homeTeam: CreateValidHomeTeam(),
                awayTeam: null,
                group: "A",
                phase: TournamentPhase.GroupStage
            ));
    }

    [TestMethod]
    public void CreateMatch_WithSameHomeAndAwayTeamReference_ShouldThrowDomainException()
    {
        var team = CreateValidHomeTeam();

        Assert.ThrowsException<DomainException>(() =>
            new Match(
                date: new DateTime(2026, 6, 1),
                stadium: CreateValidStadium(),
                homeTeam: team,
                awayTeam: team,
                group: "A",
                phase: TournamentPhase.GroupStage
            ));
    }

    [TestMethod]
    public void CreateMatch_WithDifferentInstancesButSameTeamName_ShouldThrowDomainException()
    {
        var homeTeam = new Team("Argentina", Confederation.CONMEBOL, 1800);
        var awayTeam = new Team("Argentina", Confederation.CONMEBOL, 1750);

        Assert.ThrowsException<DomainException>(() =>
            new Match(
                date: new DateTime(2026, 6, 1),
                stadium: CreateValidStadium(),
                homeTeam: homeTeam,
                awayTeam: awayTeam,
                group: "A",
                phase: TournamentPhase.GroupStage
            ));
    }

    [TestMethod]
    public void CreateMatch_WithDifferentInstancesButSameTeamNameIgnoringCase_ShouldThrowDomainException()
    {
        var homeTeam = new Team("Argentina", Confederation.CONMEBOL, 1800);
        var awayTeam = new Team("argentina", Confederation.CONMEBOL, 1750);

        Assert.ThrowsException<DomainException>(() =>
            new Match(
                date: new DateTime(2026, 6, 1),
                stadium: CreateValidStadium(),
                homeTeam: homeTeam,
                awayTeam: awayTeam,
                group: "A",
                phase: TournamentPhase.GroupStage
            ));
    }

    [TestMethod]
    public void CreateMatch_WithInvalidGroupLowercase_ShouldThrowDomainException()
    {
        Assert.ThrowsException<DomainException>(() =>
            new Match(
                date: new DateTime(2026, 6, 1),
                stadium: CreateValidStadium(),
                homeTeam: CreateValidHomeTeam(),
                awayTeam: CreateValidAwayTeam(),
                group: "a",
                phase: TournamentPhase.GroupStage
            ));
    }

    [TestMethod]
    public void CreateMatch_WithInvalidGroupLength_ShouldThrowDomainException()
    {
        Assert.ThrowsException<DomainException>(() =>
            new Match(
                date: new DateTime(2026, 6, 1),
                stadium: CreateValidStadium(),
                homeTeam: CreateValidHomeTeam(),
                awayTeam: CreateValidAwayTeam(),
                group: "AA",
                phase: TournamentPhase.GroupStage
            ));
    }

    [TestMethod]
    public void CreateMatch_WithGroupOutOfRange_ShouldThrowDomainException()
    {
        Assert.ThrowsException<DomainException>(() =>
            new Match(
                date: new DateTime(2026, 6, 1),
                stadium: CreateValidStadium(),
                homeTeam: CreateValidHomeTeam(),
                awayTeam: CreateValidAwayTeam(),
                group: "M",
                phase: TournamentPhase.GroupStage
            ));
    }

    [TestMethod]
    public void CreateGroupStageMatch_WithoutGroup_ShouldThrowDomainException()
    {
        Assert.ThrowsException<DomainException>(() =>
            new Match(
                date: new DateTime(2026, 6, 1),
                stadium: CreateValidStadium(),
                homeTeam: CreateValidHomeTeam(),
                awayTeam: CreateValidAwayTeam(),
                group: null,
                phase: TournamentPhase.GroupStage
            ));
    }

    [TestMethod]
    public void CreateKnockoutMatch_WithGroupAssigned_ShouldThrowDomainException()
    {
        Assert.ThrowsException<DomainException>(() =>
            new Match(
                date: new DateTime(2026, 7, 1),
                stadium: CreateValidStadium(),
                homeTeam: CreateValidHomeTeam(),
                awayTeam: CreateValidAwayTeam(),
                group: "A",
                phase: TournamentPhase.RoundOf32
            ));
    }

    [TestMethod]
    public void CreateMatch_ShouldGenerateIncrementalId()
    {
        var firstMatch = new Match(
            date: new DateTime(2026, 6, 1),
            stadium: CreateValidStadium(),
            homeTeam: CreateValidHomeTeam(),
            awayTeam: CreateValidAwayTeam(),
            group: "A",
            phase: TournamentPhase.GroupStage
        );

        var secondMatch = new Match(
            date: new DateTime(2026, 6, 2),
            stadium: CreateValidStadium(),
            homeTeam: new Team("Uruguay", Confederation.CONMEBOL, 1700),
            awayTeam: new Team("Chile", Confederation.CONMEBOL, 1650),
            group: "B",
            phase: TournamentPhase.GroupStage
        );

        Assert.AreEqual(firstMatch.Id + 1, secondMatch.Id);
    }

    [TestMethod]
    public void DefaultValues_AreNullOrFalse()
    {
        var match = CreateValidGroupStageMatch();

        Assert.IsNull(match.HomeGoals);
        Assert.IsNull(match.AwayGoals);
        Assert.IsNull(match.Winner);
        Assert.IsFalse(match.IsLocked);
    }

    [TestMethod]
    public void SetResult_WithHomeGoalsGreaterThanAwayGoals_ShouldSetHomeTeamAsWinner()
    {
        var match = CreateValidGroupStageMatch();

        match.SetResult(3, 1);

        Assert.AreEqual(match.HomeTeam, match.Winner);
    }

    [TestMethod]
    public void SetResult_WithAwayGoalsGreaterThanHomeGoals_ShouldSetAwayTeamAsWinner()
    {
        var match = CreateValidGroupStageMatch();

        match.SetResult(1, 2);

        Assert.AreEqual(match.AwayTeam, match.Winner);
    }

    [TestMethod]
    public void SetResult_WithDraw_ShouldSetWinnerToNull()
    {
        var match = CreateValidGroupStageMatch();

        match.SetResult(1, 1);

        Assert.IsNull(match.Winner);
    }

    [TestMethod]
    public void SetResult_WithZeroGoals_ShouldSetWinnerToNull()
    {
        var match = CreateValidGroupStageMatch();

        match.SetResult(0, 0);

        Assert.AreEqual(0, match.HomeGoals);
        Assert.AreEqual(0, match.AwayGoals);
        Assert.IsNull(match.Winner);
    }

    [TestMethod]
    public void SetResult_WithNegativeHomeGoals_ShouldThrowDomainException()
    {
        var match = CreateValidGroupStageMatch();

        Assert.ThrowsException<DomainException>(() => match.SetResult(-1, 2));
    }

    [TestMethod]
    public void SetResult_WithNegativeAwayGoals_ShouldThrowDomainException()
    {
        var match = CreateValidGroupStageMatch();

        Assert.ThrowsException<DomainException>(() => match.SetResult(2, -1));
    }

    [TestMethod]
    public void SetResult_MultipleCalls_ShouldUpdateWinnerAndGoals()
    {
        var match = CreateValidGroupStageMatch();

        match.SetResult(2, 1);
        Assert.AreEqual(match.HomeTeam, match.Winner);

        match.SetResult(1, 2);
        Assert.AreEqual(match.AwayTeam, match.Winner);

        match.SetResult(1, 1);
        Assert.IsNull(match.Winner);
    }

    [TestMethod]
    public void UpdateDate_WithValidDate_ShouldSucceed()
    {
        var match = CreateValidGroupStageMatch();
        var newDate = new DateTime(2026, 6, 3);

        match.UpdateDate(newDate);

        Assert.AreEqual(newDate, match.Date);
    }

    [TestMethod]
    public void UpdateDate_WithInvalidDate_ShouldThrowDomainException()
    {
        var match = CreateValidGroupStageMatch();

        Assert.ThrowsException<DomainException>(() =>
            match.UpdateDate(DateTime.MinValue));
    }

    [TestMethod]
    public void UpdateDate_WhenMatchIsLocked_ShouldThrowDomainException()
    {
        var match = CreateValidGroupStageMatch();
        match.LockEdition();

        Assert.ThrowsException<DomainException>(() =>
            match.UpdateDate(new DateTime(2026, 6, 4)));
    }

    [TestMethod]
    public void UpdateStadium_WithValidStadium_ShouldSucceed()
    {
        var match = CreateValidGroupStageMatch();
        var newStadium = new Stadium("Maracana", "Rio de Janeiro", "Historic stadium", 78000);

        match.UpdateStadium(newStadium);

        Assert.AreEqual(newStadium, match.Stadium);
    }

    [TestMethod]
    public void UpdateStadium_WithNullStadium_ShouldThrowDomainException()
    {
        var match = CreateValidGroupStageMatch();

        Assert.ThrowsException<DomainException>(() => match.UpdateStadium(null));
    }

    [TestMethod]
    public void UpdateStadium_WhenMatchIsLocked_ShouldThrowDomainException()
    {
        var match = CreateValidGroupStageMatch();
        var newStadium = new Stadium("Maracana", "Rio de Janeiro", "Historic stadium", 78000);
        match.LockEdition();

        Assert.ThrowsException<DomainException>(() => match.UpdateStadium(newStadium));
    }

    [TestMethod]
    public void LockEdition_ShouldSetIsLockedToTrue()
    {
        var match = CreateValidGroupStageMatch();

        match.LockEdition();

        Assert.IsTrue(match.IsLocked);
    }

    [TestMethod]
    public void SetResult_WhenMatchIsLocked_ShouldThrowDomainException()
    {
        var match = CreateValidGroupStageMatch();
        match.LockEdition();

        Assert.ThrowsException<DomainException>(() => match.SetResult(2, 1));
    }
}