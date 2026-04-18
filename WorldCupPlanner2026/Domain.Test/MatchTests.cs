namespace Domain.Test;

[TestClass]
public class MatchTests
{
    private Stadium CreateValidStadium() =>
        new Stadium("Centenario", "Montevideo", "Historic stadium", 60000);

    private Team CreateValidHomeTeam() =>
        new Team("Argentina", Confederation.CONMEBOL, 1800);

    private Team CreateValidAwayTeam() =>
        new Team("Brazil", Confederation.CONMEBOL, 1750);

    private Match CreateValidMatch() =>
        new Match(
            date: new DateTime(2026, 6, 1),
            stadium: CreateValidStadium(),
            homeTeam: CreateValidHomeTeam(),
            awayTeam: CreateValidAwayTeam(),
            group: "A",
            phase: TournamentPhase.GroupStage
        );

    [TestMethod]
    public void CreateMatch_WithValidData_ShouldSucceed()
    {
        var match = CreateValidMatch();

        Assert.IsNotNull(match);
        Assert.AreEqual("A", match.Group);
        Assert.AreEqual(TournamentPhase.GroupStage, match.Phase);
    }
    
    
}