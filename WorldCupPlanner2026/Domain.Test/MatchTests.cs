namespace Domain.Test;

[TestClass]
public class MatchTests
{
    private static readonly DateTime ValidDate = new(2026, 6, 1);
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

    private Match CreateValidMatch(
        DateTime? date = null,
        Stadium? stadium = null,
        Team? homeTeam = null,
        Team? awayTeam = null)
    {
        return new Match(
            date: date ?? ValidDate,
            stadium: stadium ?? CreateValidStadium(),
            homeTeam: homeTeam ?? CreateValidHomeTeam(),
            awayTeam: awayTeam ?? CreateValidAwayTeam()
        );
    }

    [TestMethod]
    public void CreateMatch_WithValidData_ShouldSucceed()
    {
        var match = CreateValidMatch();

        Assert.IsNotNull(match);
        Assert.AreEqual(ValidDate, match.Date);
        Assert.AreEqual("CENTENARIO", match.Stadium.Name);
        Assert.AreEqual("ARGENTINA", match.HomeTeam.Name);
        Assert.AreEqual("BRAZIL", match.AwayTeam.Name);
    }

    [TestMethod]
    public void CreateMatch_WithInvalidDate_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            CreateValidMatch(date: DateTime.MinValue));
    }

    [TestMethod]
    public void CreateMatch_WithNullStadium_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Match(
                date: ValidDate,
                stadium: null,
                homeTeam: CreateValidHomeTeam(),
                awayTeam: CreateValidAwayTeam()
            ));
    }

    [TestMethod]
    public void CreateMatch_WithNullHomeTeam_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Match(
                date: ValidDate,
                stadium: CreateValidStadium(),
                homeTeam: null,
                awayTeam: CreateValidAwayTeam()
            ));
    }

    [TestMethod]
    public void CreateMatch_WithNullAwayTeam_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Match(
                date: ValidDate,
                stadium: CreateValidStadium(),
                homeTeam: CreateValidHomeTeam(),
                awayTeam: null
            ));
    }

    [TestMethod]
    public void CreateMatch_WithSameHomeAndAwayTeamReference_ShouldThrowArgumentException()
    {
        var team = CreateValidHomeTeam();

        Assert.ThrowsException<ArgumentException>(() =>
            CreateValidMatch(homeTeam: team, awayTeam: team));
    }

    [TestMethod]
    public void CreateMatch_WithDifferentInstancesButSameTeamName_ShouldThrowArgumentException()
    {
        var homeTeam = new Team("ARGENTINA", "CONMEBOL", 1800);
        var awayTeam = new Team("ARGENTINA", "CONMEBOL", 1750);

        Assert.ThrowsException<ArgumentException>(() =>
            CreateValidMatch(homeTeam: homeTeam, awayTeam: awayTeam));
    }

    [TestMethod]
    public void CreateMatch_ShouldGenerateIncrementalId()
    {
        var firstMatch = CreateValidMatch();

        var secondMatch = CreateValidMatch(
            date: new DateTime(2026, 6, 2),
            homeTeam: CreateUruguayTeam(),
            awayTeam: CreateChileTeam()
        );

        Assert.AreEqual(firstMatch.Id + 1, secondMatch.Id);
    }

    [TestMethod]
    public void CreateMatch_ShouldInitializeResultAndLockValues()
    {
        var match = CreateValidMatch();

        Assert.IsNull(match.HomeGoals);
        Assert.IsNull(match.AwayGoals);
        Assert.IsNull(match.Winner);
        Assert.IsFalse(match.IsLocked);
    }

    [TestMethod]
    public void SetResult_WithHomeTeamWinner_ShouldSetGoalsAndWinner()
    {
        var match = CreateValidMatch();

        match.SetResult(ValidHomeGoals, ValidAwayGoals);

        Assert.AreEqual(ValidHomeGoals, match.HomeGoals);
        Assert.AreEqual(ValidAwayGoals, match.AwayGoals);
        Assert.AreEqual(match.HomeTeam, match.Winner);
    }

    [TestMethod]
    public void SetResult_WithAwayTeamWinner_ShouldSetGoalsAndWinner()
    {
        var match = CreateValidMatch();

        match.SetResult(1, 2);

        Assert.AreEqual(1, match.HomeGoals);
        Assert.AreEqual(2, match.AwayGoals);
        Assert.AreEqual(match.AwayTeam, match.Winner);
    }

    [TestMethod]
    public void SetResult_WithDraw_ShouldSetWinnerAsNull()
    {
        var match = CreateValidMatch();

        match.SetResult(1, 1);

        Assert.AreEqual(1, match.HomeGoals);
        Assert.AreEqual(1, match.AwayGoals);
        Assert.IsNull(match.Winner);
    }

    [TestMethod]
    public void SetResult_WithNegativeHomeGoals_ShouldThrowArgumentOutOfRangeException()
    {
        var match = CreateValidMatch();

        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            match.SetResult(-1, 2));
    }

    [TestMethod]
    public void SetResult_WithNegativeAwayGoals_ShouldThrowArgumentOutOfRangeException()
    {
        var match = CreateValidMatch();

        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            match.SetResult(2, -1));
    }

    [TestMethod]
    public void SetResult_WhenMatchIsLocked_ShouldThrowInvalidOperationException()
    {
        var match = CreateValidMatch();
        match.LockEdition();

        Assert.ThrowsException<InvalidOperationException>(() =>
            match.SetResult(2, 1));
    }

    [TestMethod]
    public void UpdateDate_WithValidDate_ShouldUpdateDate()
    {
        var match = CreateValidMatch();

        match.UpdateDate(NewValidDate);

        Assert.AreEqual(NewValidDate, match.Date);
    }

    [TestMethod]
    public void UpdateDate_WithInvalidDate_ShouldThrowArgumentException()
    {
        var match = CreateValidMatch();

        Assert.ThrowsException<ArgumentException>(() =>
            match.UpdateDate(DateTime.MinValue));
    }

    [TestMethod]
    public void UpdateDate_WhenMatchIsLocked_ShouldThrowInvalidOperationException()
    {
        var match = CreateValidMatch();
        match.LockEdition();

        Assert.ThrowsException<InvalidOperationException>(() =>
            match.UpdateDate(NewValidDate));
    }

    [TestMethod]
    public void UpdateStadium_WithValidStadium_ShouldUpdateStadium()
    {
        var match = CreateValidMatch();
        var newStadium = CreateAnotherValidStadium();

        match.UpdateStadium(newStadium);

        Assert.AreEqual("MARACANA", match.Stadium.Name);
        Assert.AreEqual("RIO DE JANEIRO", match.Stadium.City);
    }

    [TestMethod]
    public void UpdateStadium_WithNullStadium_ShouldThrowArgumentException()
    {
        var match = CreateValidMatch();

        Assert.ThrowsException<ArgumentException>(() =>
            match.UpdateStadium(null));
    }

    [TestMethod]
    public void UpdateStadium_WhenMatchIsLocked_ShouldThrowInvalidOperationException()
    {
        var match = CreateValidMatch();
        match.LockEdition();

        Assert.ThrowsException<InvalidOperationException>(() =>
            match.UpdateStadium(CreateAnotherValidStadium()));
    }

    [TestMethod]
    public void LockEdition_ShouldSetIsLockedToTrue()
    {
        var match = CreateValidMatch();

        match.LockEdition();

        Assert.IsTrue(match.IsLocked);
    }
}