namespace Domain.Test;

using Domain.Exceptions;

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
    public void CreateMatch_WithNullHomeTeam_ShouldThrowDomainException()
    {
        Assert.ThrowsException<DomainException>(() => new Match(date: new DateTime(2026, 6, 1), stadium: CreateValidStadium(), homeTeam: null, awayTeam: CreateValidAwayTeam(), group: "A", phase: TournamentPhase.GroupStage));
    }
    
    [TestMethod]
    public void CreateMatch_WithNullAwayTeam_ShouldThrowDomainException()
    {
        Assert.ThrowsException<DomainException>(() => new Match(date: new DateTime(2026, 6, 1), stadium: CreateValidStadium(), homeTeam: CreateValidHomeTeam(), awayTeam: null, group: "A", phase: TournamentPhase.GroupStage));
    }
    
    [TestMethod]
    public void CreateMatch_WithSameHomeAndAwayTeam_ShouldThrowDomainException()
    {
        var team = CreateValidHomeTeam();

        Assert.ThrowsException<DomainException>(() => new Match(date: new DateTime(2026, 6, 1), stadium: CreateValidStadium(), homeTeam: team, awayTeam: team, group: "A", phase: TournamentPhase.GroupStage));
    }
    
    [TestMethod]
    public void CreateMatch_WithInvalidGroupLowercase_ShouldThrowDomainException()
    {
        Assert.ThrowsException<DomainException>(() => new Match(date: new DateTime(2026, 6, 1), stadium: CreateValidStadium(), homeTeam: CreateValidHomeTeam(), awayTeam: CreateValidAwayTeam(), group: "a", phase: TournamentPhase.GroupStage));
    }
    
    [TestMethod]
    public void SetResult_WithNegativeHomeGoals_ShouldThrowDomainException()
    {
        var match = CreateValidMatch();

        Assert.ThrowsException<DomainException>(() => match.SetResult(-1, 2));
    }
    
    [TestMethod]
    public void CreateMatch_ShouldGenerateIncrementalId()
    {
        var firstMatch = new Match(date: new DateTime(2026, 6, 1), stadium: CreateValidStadium(), homeTeam: CreateValidHomeTeam(), awayTeam: CreateValidAwayTeam(), group: "A", phase: TournamentPhase.GroupStage);

        var secondMatch = new Match(date: new DateTime(2026, 6, 2), stadium: CreateValidStadium(), homeTeam: new Team("Uruguay", Confederation.CONMEBOL, 1700), awayTeam: new Team("Chile", Confederation.CONMEBOL, 1650), group: "B", phase: TournamentPhase.GroupStage);

        Assert.AreEqual(firstMatch.Id + 1, secondMatch.Id);
    }
    
    [TestMethod]
    public void UpdateDate_WithValidDate_ShouldSucceed()
    {
        var match = CreateValidMatch();
        var newDate = new DateTime(2026, 6, 3);

        match.UpdateDate(newDate);

        Assert.AreEqual(newDate, match.Date);
    }
    
    [TestMethod]
    public void UpdateStadium_WithValidStadium_ShouldSucceed()
    {
        var match = CreateValidMatch();
        var newStadium = new Stadium("Maracana", "Rio de Janeiro", "Historic stadium", 78000);

        match.UpdateStadium(newStadium);

        Assert.AreEqual(newStadium, match.Stadium);
    }
    
    [TestMethod]
    public void UpdateDate_WhenMatchIsLocked_ShouldThrowDomainException()
    {
        var match = CreateValidMatch();
        match.LockEdition();

        Assert.ThrowsException<DomainException>(() => match.UpdateDate(new DateTime(2026, 6, 4)));
    }
    
    
}