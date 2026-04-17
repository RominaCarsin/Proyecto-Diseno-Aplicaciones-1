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
}