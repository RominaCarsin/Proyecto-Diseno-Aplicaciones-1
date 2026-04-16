namespace Domain.Test;

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
    
    
}