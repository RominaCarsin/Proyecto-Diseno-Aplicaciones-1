namespace Domain.Test;

[TestClass]
public class MatchTests
{
    private Stadium CreateValidStadium() => new Stadium("Centenario", "Montevideo", "Historic stadium", 60000);

    private Team CreateValidHomeTeam() => new Team("Argentina",Confederation.CONMEBOL, 1800);

    private Team CreateValidAwayTeam() => new Team("Brazil", Confederation.CONMEBOL, 1750);

    private Match CreateValidMatch() => new Match(date: new DateTime(2026, 6, 1), stadium: CreateValidStadium(), homeTeam: CreateValidHomeTeam(), awayTeam: CreateValidAwayTeam(), group: "A", phase: TournamentPhase.GroupStage);

    [TestMethod]
    public void CreateMatch_WithValidData_ShouldSucceed()
    {
        var match = CreateValidMatch();

        Assert.IsNotNull(match);
        Assert.AreEqual("A", match.Group);
        Assert.AreEqual(TournamentPhase.GroupStage, match.Phase);
    }
    
    //estos siempre dieron bien
    [TestMethod]
    public void CreateMatch_WithGroupA_ShouldSucceed()
    {
        var match = new Match(date: new DateTime(2026, 6, 1), stadium: CreateValidStadium(), homeTeam: CreateValidHomeTeam(), awayTeam: CreateValidAwayTeam(), group: "A", phase: TournamentPhase.GroupStage);

        Assert.AreEqual("A", match.Group);
    }
    
    [TestMethod]
    public void CreateMatch_WithGroupL_ShouldSucceed()
    {
        var match = new Match(date: new DateTime(2026, 6, 1), stadium: CreateValidStadium(), homeTeam: CreateValidHomeTeam(), awayTeam: CreateValidAwayTeam(), group: "L", phase: TournamentPhase.GroupStage);

        Assert.AreEqual("L", match.Group);
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
    public void LoadResult_WithValidData_ShouldSucceed()
    {
        var match = CreateValidMatch();

        match.LoadResult(2, 1);

        Assert.AreEqual(2, match.HomeGoals);
        Assert.AreEqual(1, match.AwayGoals);
    }
}