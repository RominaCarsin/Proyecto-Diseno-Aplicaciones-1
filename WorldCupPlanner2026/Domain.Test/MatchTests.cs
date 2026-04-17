namespace Domain.Test;

[TestClass]
public class MatchTests
{
    [TestMethod]
    public void CreateMatch_WithValidData_ShouldSucceed()
    {
        var stadium = new Stadium("Centenario", "Montevideo", "Historic stadium", 60000);
        var homeTeam = new Team("Uruguay");
        var awayTeam = new Team("Argentina");
        var group = new Group('A');

        var match = new Match(
            new DateTime(2026, 6, 1, 14, 0, 0),
            stadium,
            homeTeam,
            awayTeam,
            group,
            TournamentPhase.GroupStage);

        Assert.IsNotNull(match);
        Assert.AreEqual(stadium, match.Stadium);
        Assert.AreEqual(homeTeam, match.HomeTeam);
        Assert.AreEqual(awayTeam, match.AwayTeam);
        Assert.AreEqual(group, match.Group);
        Assert.AreEqual(TournamentPhase.GroupStage, match.Phase);
    }
    
    [TestMethod]
    public void CreateMatch_WithNullStadium_ShouldThrowException()
    {
        var homeTeam = new Team("Uruguay");
        var awayTeam = new Team("Argentina");
        var group = new Group('A');

        Assert.ThrowsException<Exception>(() => new Match(DateTime.Now, null, homeTeam, awayTeam, group, TournamentPhase.GroupStage));
    }
    
    [TestMethod]
    public void CreateMatch_WithNullHomeTeam_ShouldThrowException()
    {
        var stadium = new Stadium("Centenario", "Montevideo", "Historic stadium", 60000);
        var awayTeam = new Team("Argentina");
        var group = new Group('A');

        Assert.ThrowsException<Exception>(() => new Match( DateTime.Now, stadium, null, awayTeam, group, TournamentPhase.GroupStage));
    }
    
    [TestMethod]
    public void CreateMatch_WithNullAwayTeam_ShouldThrowException()
    {
        var stadium = new Stadium("Centenario", "Montevideo", "Historic stadium", 60000);
        var homeTeam = new Team("Uruguay");
        var group = new Group('A');

        Assert.ThrowsException<Exception>(() => new Match(DateTime.Now, stadium, homeTeam, null, group, TournamentPhase.GroupStage));
    }
    
    [TestMethod]
    public void CreateMatch_WithSameTeams_ShouldThrowException()
    {
        var stadium = new Stadium("Centenario", "Montevideo", "Historic stadium", 60000);
        var team = new Team("Uruguay");
        var group = new Group('A');

        Assert.ThrowsException<Exception>(() => new Match(DateTime.Now, stadium, team, team, group, TournamentPhase.GroupStage));
    }
    
    [TestMethod]
    public void CreateMatch_WithNullGroup_ShouldThrowException()
    {
        var stadium = new Stadium("Centenario", "Montevideo", "Historic stadium", 60000);
        var homeTeam = new Team("Uruguay");
        var awayTeam = new Team("Argentina");

        Assert.ThrowsException<Exception>(() => new Match(DateTime.Now, stadium, homeTeam, awayTeam, null, TournamentPhase.GroupStage));
    }
    
    [TestMethod]//El if lo hice en Group.cs
    public void CreateMatch_WithInvalidGroupLabel_ShouldThrowException()
    {
        var stadium = new Stadium("Centenario", "Montevideo", "Historic stadium", 60000);
        var homeTeam = new Team("Uruguay");
        var awayTeam = new Team("Argentina");

        Assert.ThrowsException<Exception>(() => new Match(DateTime.Now, stadium, homeTeam, awayTeam, new Group('Z'), TournamentPhase.GroupStage));
    }
    
    [TestMethod] //no me dio error esto se borra
    public void CreateMatch_WithPlayoffsPhase_ShouldSucceed()
    {
        var stadium = new Stadium("Centenario", "Montevideo", "Historic stadium", 60000);
        var homeTeam = new Team("Uruguay");
        var awayTeam = new Team("Argentina");
        var group = new Group('A');

        var match = new Match(DateTime.Now, stadium, homeTeam, awayTeam, group, TournamentPhase.Playoffs);

        Assert.IsNotNull(match);
        Assert.AreEqual(TournamentPhase.Playoffs, match.Phase);
    }
    
    
    [TestMethod]
    public void LoadResult_WithValidData_ShouldSucceed()
    {
        var match = CreateValidMatch();

        match.LoadResult(2, 1);

        Assert.AreEqual(2, match.HomeGoals);
        Assert.AreEqual(1, match.AwayGoals);
    }
    
    
}