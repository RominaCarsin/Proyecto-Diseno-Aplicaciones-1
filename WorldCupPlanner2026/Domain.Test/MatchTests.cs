namespace Domain.Test;

[TestClass]
public class MatchTests
{
    private static readonly DateTime ValidGroupStageDate = new(2026, 6, 1);
    private static readonly DateTime ValidKnockoutDate = new(2026, 7, 1);
    private static readonly DateTime NewValidDate = new(2026, 6, 10);

    private const int ValidHomeGoals = 3;
    private const int ValidAwayGoals = 1;

    private Stadium CreateValidStadium() =>
        new Stadium("CENTENARIO", "MONTEVIDEO", "Historic stadium", 60000);

    private Stadium CreateAnotherValidStadium() =>
        new Stadium("MARACANA", "RIO DE JANEIRO", "Historic stadium", 78000);

    private Team CreateValidHomeTeam() =>
        new Team("ARGENTINA", "CONMEBOL", 1800);

    private Team CreateValidAwayTeam() =>
        new Team("BRAZIL", "CONMEBOL", 1750);

    private Team CreateUruguayTeam() =>
        new Team("URUGUAY", "CONMEBOL", 1700);

    private Team CreateChileTeam() =>
        new Team("CHILE", "CONMEBOL", 1650);

    private Group CreateGroupA() =>
        new Group('A');

    private Group CreateGroupB() =>
        new Group('B');

    private Match CreateGroupStageMatch(
        DateTime? date = null,
        Stadium? stadium = null,
        Team? homeTeam = null,
        Team? awayTeam = null,
        Group? group = null)
    {
        return new Match(
            date: date ?? ValidGroupStageDate,
            stadium: stadium ?? CreateValidStadium(),
            homeTeam: homeTeam ?? CreateValidHomeTeam(),
            awayTeam: awayTeam ?? CreateValidAwayTeam(),
            group: group ?? CreateGroupA(),
            phase: TournamentPhase.GroupStage
        );
    }

    private Match CreateKnockoutMatch(
        DateTime? date = null,
        Stadium? stadium = null,
        Team? homeTeam = null,
        Team? awayTeam = null,
        Group? group = null,
        TournamentPhase phase = TournamentPhase.RoundOf32)
    {
        return new Match(
            date: date ?? ValidKnockoutDate,
            stadium: stadium ?? CreateValidStadium(),
            homeTeam: homeTeam ?? CreateValidHomeTeam(),
            awayTeam: awayTeam ?? CreateValidAwayTeam(),
            group: group,
            phase: phase
        );
    }

    [TestMethod]
    public void CreateMatch_WithValidGroupStageData_ShouldSucceed()
    {
        var match = CreateGroupStageMatch();

        Assert.IsNotNull(match);
        Assert.AreEqual(ValidGroupStageDate, match.Date);
        Assert.AreEqual("CENTENARIO", match.Stadium.Name);
        Assert.AreEqual("ARGENTINA", match.HomeTeam.Name);
        Assert.AreEqual("BRAZIL", match.AwayTeam.Name);
        Assert.AreEqual("A", match.Group!.ToString());
        Assert.AreEqual(TournamentPhase.GroupStage, match.Phase);
    }

    [TestMethod]
    public void CreateMatch_WithValidKnockoutData_ShouldSucceed()
    {
        var match = CreateKnockoutMatch();

        Assert.IsNotNull(match);
        Assert.IsNull(match.Group);
        Assert.AreEqual(TournamentPhase.RoundOf32, match.Phase);
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

    [TestMethod]
    public void CreateMatch_ShouldGenerateIncrementalId()
    {
        var firstMatch = CreateGroupStageMatch();

        var secondMatch = CreateGroupStageMatch(
            date: new DateTime(2026, 6, 2),
            homeTeam: CreateUruguayTeam(),
            awayTeam: CreateChileTeam(),
            group: CreateGroupB()
        );

        Assert.AreEqual(firstMatch.Id + 1, secondMatch.Id);
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
    public void SetResult_WithHomeTeamWinner_ShouldSetGoalsAndWinner()
    {
        var match = CreateGroupStageMatch();

        match.SetResult(ValidHomeGoals, ValidAwayGoals);

        Assert.AreEqual(ValidHomeGoals, match.HomeGoals);
        Assert.AreEqual(ValidAwayGoals, match.AwayGoals);
        Assert.AreEqual(match.HomeTeam, match.Winner);
    }

    [TestMethod]
    public void SetResult_WithAwayTeamWinner_ShouldSetGoalsAndWinner()
    {
        var match = CreateGroupStageMatch();

        match.SetResult(1, 2);

        Assert.AreEqual(1, match.HomeGoals);
        Assert.AreEqual(2, match.AwayGoals);
        Assert.AreEqual(match.AwayTeam, match.Winner);
    }

    [TestMethod]
    public void SetResult_WithDrawInGroupStage_ShouldSetWinnerAsNull()
    {
        var match = CreateGroupStageMatch();

        match.SetResult(1, 1);

        Assert.AreEqual(1, match.HomeGoals);
        Assert.AreEqual(1, match.AwayGoals);
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
        var match = CreateGroupStageMatch();
        match.LockEdition();

        Assert.ThrowsException<InvalidOperationException>(() =>
            match.SetResult(2, 1));
    }

    [TestMethod]
    public void UpdateDate_WithValidDate_ShouldUpdateDate()
    {
        var match = CreateGroupStageMatch();

        match.UpdateDate(NewValidDate);

        Assert.AreEqual(NewValidDate, match.Date);
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
        var match = CreateGroupStageMatch();
        match.LockEdition();

        Assert.ThrowsException<InvalidOperationException>(() =>
            match.UpdateDate(NewValidDate));
    }

    [TestMethod]
    public void UpdateStadium_WithValidStadium_ShouldUpdateStadium()
    {
        var match = CreateGroupStageMatch();
        var newStadium = CreateAnotherValidStadium();

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
        var match = CreateGroupStageMatch();
        match.LockEdition();

        Assert.ThrowsException<InvalidOperationException>(() =>
            match.UpdateStadium(CreateAnotherValidStadium()));
    }

    [TestMethod]
    public void LockEdition_ShouldSetIsLockedToTrue()
    {
        var match = CreateGroupStageMatch();

        match.LockEdition();

        Assert.IsTrue(match.IsLocked);
    }
}
