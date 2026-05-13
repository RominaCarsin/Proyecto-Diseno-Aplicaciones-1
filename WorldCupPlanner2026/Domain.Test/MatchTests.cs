namespace Domain.Test;

[TestClass]
public class MatchTests
{
    private static readonly DateTime ValidDate = new(2026, 6, 1);
    private static readonly DateTime AnotherValidDate = new(2026, 6, 10);

    private const int ValidHomeGoals = 3;
    private const int ValidAwayGoals = 1;
    

    [TestMethod]
    public void CreateMatch_WithValidData_ShouldCreateMatchWithExpectedProperties()
    {
        var match = CreateValidMatch();

        Assert.AreEqual(ValidDate, match.Date);
        Assert.AreEqual("CENTENARIO", match.Stadium.Name);
        Assert.AreEqual("ARGENTINA", match.HomeTeam.Name);
        Assert.AreEqual("BRAZIL", match.AwayTeam.Name);
    }

    [TestMethod]
    public void CreateMatch_ShouldInitializeWithNoResultAndUnlocked()
    {
        var match = CreateValidMatch();

        Assert.IsNull(match.HomeGoals);
        Assert.IsNull(match.AwayGoals);
        Assert.IsNull(match.Winner);
        Assert.IsFalse(match.IsLocked);
    }
    
    [TestMethod]
    public void CreateMatch_WithValidData_ShouldSetStatusAsNotFinished()
    {
        var match = CreateValidMatch();

        Assert.AreEqual(MatchStatus.NotFinished, match.Status);
    }
    
    [TestMethod]
    public void Finish_WithValidMatch_ShouldSetStatusAsFinished()
    {
        var match = CreateValidMatch();

        match.Finish();

        Assert.AreEqual(MatchStatus.Finished, match.Status);
    }
    
    [TestMethod]
    public void Finish_WithValidMatch_ShouldLockMatch()
    {
        var match = CreateValidMatch();

        match.Finish();

        Assert.IsTrue(match.IsLocked);
    }
    
    [TestMethod]
    public void Finish_WithFinishedMatch_ShouldThrowInvalidOperationException()
    {
        var match = CreateValidMatch();

        match.Finish();

        Assert.ThrowsException<InvalidOperationException>(() =>
            match.Finish());
    }
    
    [TestMethod]
    public void SetResult_WithExistingResult_ShouldRevertPreviousTeamStats()
    {
        Team homeTeam = BuildTeam(name: "URUGUAY");
        Team awayTeam = BuildTeam(name: "ARGENTINA");

        Match match = CreateValidMatch(
            homeTeam: homeTeam,
            awayTeam: awayTeam);

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
    public void SetResult_WhenMatchIsLockedButNotFinished_ShouldSetResult()
    {
        Match match = CreateValidMatch();

        match.LockEdition();

        match.SetResult(2, 1);

        Assert.AreEqual(2, match.HomeGoals);
        Assert.AreEqual(1, match.AwayGoals);
        Assert.AreEqual(match.HomeTeam, match.Winner);
    }
    
    [TestMethod]
    public void UpdateDate_WhenMatchIsLockedButNotFinished_ShouldReplaceMatchDate()
    {
        Match match = CreateValidMatch();

        match.LockEdition();

        match.UpdateDate(AnotherValidDate);

        Assert.AreEqual(AnotherValidDate, match.Date);
    }
    
    [TestMethod]
    public void UpdateStadium_WhenMatchIsLockedButNotFinished_ShouldReplaceMatchStadium()
    {
        Match match = CreateValidMatch();
        Stadium newStadium = BuildStadium(name: "MARACANA", city: "RIO DE JANEIRO");

        match.LockEdition();

        match.UpdateStadium(newStadium);

        Assert.AreEqual("MARACANA", match.Stadium.Name);
        Assert.AreEqual("RIO DE JANEIRO", match.Stadium.City);
    }

    [TestMethod]
    public void CreateMatch_ShouldGenerateIncrementalId()
    {
        var firstMatch = CreateValidMatch();
        var secondMatch = CreateValidMatch(
            homeTeam: BuildTeam(name: "URUGUAY"),
            awayTeam: BuildTeam(name: "CHILE")
        );

        Assert.AreEqual(firstMatch.Id + 1, secondMatch.Id);
    }

    [TestMethod]
    public void CreateMatch_WithDifferentTeams_ShouldSucceed()
    {
        var homeTeam = BuildTeam(name: "ARGENTINA");
        var awayTeam = BuildTeam(name: "BRAZIL");

        var match = CreateValidMatch(homeTeam: homeTeam, awayTeam: awayTeam);

        Assert.AreEqual("ARGENTINA", match.HomeTeam.Name);
        Assert.AreEqual("BRAZIL", match.AwayTeam.Name);
    }

    [TestMethod]
    public void CreateMatch_WithMinValueDate_ShouldThrowArgumentException()
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
                homeTeam: BuildTeam(name: "ARGENTINA"),
                awayTeam: BuildTeam(name: "BRAZIL")
            ));
    }

    [TestMethod]
    public void CreateMatch_WithNullHomeTeam_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Match(
                date: ValidDate,
                stadium: BuildStadium(name: "CENTENARIO", city: "MONTEVIDEO"),
                homeTeam: null,
                awayTeam: BuildTeam(name: "BRAZIL")
            ));
    }

    [TestMethod]
    public void CreateMatch_WithNullAwayTeam_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Match(
                date: ValidDate,
                stadium: BuildStadium(name: "CENTENARIO", city: "MONTEVIDEO"),
                homeTeam: BuildTeam(name: "ARGENTINA"),
                awayTeam: null
            ));
    }

    [TestMethod]
    public void CreateMatch_WithSameTeamReferenceForBothRoles_ShouldThrowArgumentException()
    {
        var sameTeam = BuildTeam(name: "ARGENTINA");

        Assert.ThrowsException<ArgumentException>(() =>
            CreateValidMatch(homeTeam: sameTeam, awayTeam: sameTeam));
    }

    [TestMethod]
    public void CreateMatch_WithDifferentInstancesOfSameTeamName_ShouldThrowArgumentException()
    {
        var homeTeam = BuildTeam(name: "ARGENTINA");
        var awayTeamWithSameName = BuildTeam(name: "ARGENTINA");

        Assert.ThrowsException<ArgumentException>(() =>
            CreateValidMatch(homeTeam: homeTeam, awayTeam: awayTeamWithSameName));
    }


    [TestMethod]
    public void SetResult_WithHomeTeamScoringMore_ShouldSetHomeTeamAsWinner()
    {
        var match = CreateValidMatch();
        int homeGoals = 3, awayGoals = 1;

        match.SetResult(homeGoals, awayGoals);

        Assert.AreEqual(homeGoals, match.HomeGoals);
        Assert.AreEqual(awayGoals, match.AwayGoals);
        Assert.AreEqual(match.HomeTeam, match.Winner);
    }

    [TestMethod]
    public void SetResult_WithAwayTeamScoringMore_ShouldSetAwayTeamAsWinner()
    {
        var match = CreateValidMatch();
        int homeGoals = 1, awayGoals = 2;

        match.SetResult(homeGoals, awayGoals);

        Assert.AreEqual(homeGoals, match.HomeGoals);
        Assert.AreEqual(awayGoals, match.AwayGoals);
        Assert.AreEqual(match.AwayTeam, match.Winner);
    }

    [TestMethod]
    public void SetResult_WithEqualGoals_ShouldSetWinnerAsNull()
    {
        var match = CreateValidMatch();
        int homeGoals = 1, awayGoals = 1;

        match.SetResult(homeGoals, awayGoals);

        Assert.AreEqual(homeGoals, match.HomeGoals);
        Assert.AreEqual(awayGoals, match.AwayGoals);
        Assert.IsNull(match.Winner);
    }

    [TestMethod]
    public void SetResult_WithZeroGoalsDraw_ShouldSetWinnerAsNull()
    {
        var match = CreateValidMatch();

        match.SetResult(homeGoals: 0, awayGoals: 0);

        Assert.AreEqual(0, match.HomeGoals);
        Assert.AreEqual(0, match.AwayGoals);
        Assert.IsNull(match.Winner);
    }

    [TestMethod]
    public void SetResult_WithHomeGoalsZeroAndAwayGoalsOne_ShouldSetAwayTeamAsWinner()
    {
        var match = CreateValidMatch();

        match.SetResult(homeGoals: 0, awayGoals: 1);

        Assert.AreEqual(match.AwayTeam, match.Winner);
    }

    [TestMethod]
    public void SetResult_WithHomeGoalsOneAndAwayGoalsZero_ShouldSetHomeTeamAsWinner()
    {
        var match = CreateValidMatch();

        match.SetResult(homeGoals: 1, awayGoals: 0);

        Assert.AreEqual(match.HomeTeam, match.Winner);
    }

    [TestMethod]
    public void SetResult_WithNegativeHomeGoals_ShouldThrowArgumentOutOfRangeException()
    {
        var match = CreateValidMatch();

        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            match.SetResult(homeGoals: -1, awayGoals: 2));
    }

    [TestMethod]
    public void SetResult_WithNegativeAwayGoals_ShouldThrowArgumentOutOfRangeException()
    {
        var match = CreateValidMatch();

        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            match.SetResult(homeGoals: 2, awayGoals: -1));
    }

    [TestMethod]
    public void SetResult_WhenMatchIsLocked_ShouldThrowInvalidOperationException()
    {
        var lockedMatch = CreateLockedMatch();

        Assert.ThrowsException<InvalidOperationException>(() =>
            lockedMatch.SetResult(ValidHomeGoals, ValidAwayGoals));
    }


    [TestMethod]
    public void UpdateDate_WithValidDate_ShouldReplaceMatchDate()
    {
        var match = CreateValidMatch();

        match.UpdateDate(AnotherValidDate);

        Assert.AreEqual(AnotherValidDate, match.Date);
    }

    [TestMethod]
    public void UpdateDate_WithMinValueDate_ShouldThrowArgumentException()
    {
        var match = CreateValidMatch();

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


    [TestMethod]
    public void UpdateStadium_WithValidStadium_ShouldReplaceMatchStadium()
    {
        var match = CreateValidMatch();
        var newStadium = BuildStadium(name: "MARACANA", city: "RIO DE JANEIRO");

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
        var lockedMatch = CreateLockedMatch();
        var newStadium = BuildStadium(name: "MARACANA", city: "RIO DE JANEIRO");

        Assert.ThrowsException<InvalidOperationException>(() =>
            lockedMatch.UpdateStadium(newStadium));
    }


    [TestMethod]
    public void LockEdition_ShouldSetIsLockedToTrue()
    {
        var match = CreateValidMatch();

        match.LockEdition();

        Assert.IsTrue(match.IsLocked);
    }
    
    [TestMethod]
    public void SetResult_WhenHomeTeamWins_ShouldUpdateTeamsStats()
    {
        Team homeTeam = BuildTeam(name: "URUGUAY");
        Team awayTeam = BuildTeam(name: "ARGENTINA");

        Match match = CreateValidMatch(
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


    private static Match CreateValidMatch(
        DateTime? date = null,
        Stadium? stadium = null,
        Team? homeTeam = null,
        Team? awayTeam = null) =>
        new Match(
            date: date ?? ValidDate,
            stadium: stadium ?? BuildStadium(name: "CENTENARIO", city: "MONTEVIDEO"),
            homeTeam: homeTeam ?? BuildTeam(name: "ARGENTINA"),
            awayTeam: awayTeam ?? BuildTeam(name: "BRAZIL")
        );

    private static Match CreateLockedMatch()
    {
        var match = CreateValidMatch();
        match.LockEdition();
        return match;
    }

    private static Stadium BuildStadium(string name, string city = "MONTEVIDEO") =>
        new Stadium(name, city, "Historic stadium", 60000);

    private static Team BuildTeam(string name) =>
        new Team(name, "CONMEBOL", 1800);
}