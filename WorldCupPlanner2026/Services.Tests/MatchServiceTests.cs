using Domain;
using IRepository;
using IServices;
using Repository;
using Services;

namespace Services.Tests;

[TestClass]
public class MatchServiceTests
{
    private MatchService _matchService = null!;

    [TestInitialize]
    public void Setup()
    {
        IRepository<Match> repository = new Repository<Match>();
        _matchService = new MatchService(repository);
        
    }
    
    private Stadium CreateValidStadium()
    {
        return new Stadium("CENTENARIO", "MONTEVIDEO", "Historic stadium", 60000);
    }

    private Team CreateValidHomeTeam()
    {
        return new Team("ARGENTINA", "CONMEBOL", 1800);
    }

    private Team CreateValidAwayTeam()
    {
        return new Team("BRAZIL", "CONMEBOL", 1750);
    }

    private Match CreateValidGroupStageMatch()
    {
        return new Match(
            date: new DateTime(2026, 6, 1),
            stadium: CreateValidStadium(),
            homeTeam: CreateValidHomeTeam(),
            awayTeam: CreateValidAwayTeam(),
            group: new Group('A'),
            phase: TournamentPhase.GroupStage
        );
    }
    
    [TestMethod]
    public void AddMatch_WithValidMatch_ShouldAddMatch()
    {
        var match = CreateValidGroupStageMatch();

        _matchService.AddMatch(match);

        Assert.AreEqual(1, _matchService.GetMatches().Count());
    }
}