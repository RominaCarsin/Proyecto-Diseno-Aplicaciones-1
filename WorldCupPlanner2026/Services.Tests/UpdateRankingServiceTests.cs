using Domain;
using IRepository;
using Services;

namespace Services.Tests;

[TestClass]
public class UpdateRankingServiceTests
{
    private static readonly DateTime ValidDate = new(2026, 6, 1);

    private UpdateRankingService _service = null!;
    private IRepository<AuditLog> _auditRepository = null!;
    private AuditService _auditService = null!;

    [TestInitialize]
    public void Setup()
    {
        _auditRepository = new FakeRepository<AuditLog>();
        _auditService = new AuditService(_auditRepository);
        _service = new UpdateRankingService(_auditService);
    }

    [TestMethod]
    public void UpdateAfterMatch_WhenWeakTeamWinsInGroupStage_ShouldUpdateBothRankings()
    {
        var strong = BuildTeam("STRONG", 1500);
        var weak = BuildTeam("WEAK", 1200);
        var match = BuildFinishedMatch(home: weak, away: strong, homeGoals: 2, awayGoals: 0);

        _service.UpdateAfterMatch(match);

        Assert.AreEqual(1220, weak.RankingFifa);
        Assert.AreEqual(1480, strong.RankingFifa);
    }
    
    [TestMethod]
    public void UpdateAfterMatch_WhenMatchIsNotFinished_ShouldThrowInvalidOperationException()
    {
        var strong = BuildTeam("STRONG", 1500);
        var weak = BuildTeam("WEAK", 1200);

        var match = new Match(
            date: ValidDate,
            stadium: new Stadium("CENTENARIO", "MONTEVIDEO", "Historic stadium", 60000),
            homeTeam: weak,
            awayTeam: strong,
            group: 'A',
            phase: TournamentPhase.GroupStage);
        match.SetResult(2, 0);   // tiene resultado pero NO está finalizado

        Assert.ThrowsException<InvalidOperationException>(() =>
            _service.UpdateAfterMatch(match));
    }
    
    [TestMethod]
    public void UpdateAfterMatch_ShouldAuditRankingChangeForBothTeams()
    {
        var strong = BuildTeam("STRONG", 1500);
        var weak = BuildTeam("WEAK", 1200);
        var match = BuildFinishedMatch(home: weak, away: strong, homeGoals: 2, awayGoals: 0);

        _service.UpdateAfterMatch(match);

        var logs = _auditRepository.GetAll().ToList();
        // Una entrada de auditoría por cada equipo cuyo ranking cambió.
        Assert.AreEqual(2, logs.Count);
        Assert.IsTrue(logs.All(log => log.Action.Contains("RANKING", StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public void UpdateAfterMatch_WhenRankingAlreadyApplied_ShouldNotRecalculateOrAuditAgain()
    {
        var strong = BuildTeam("STRONG", 1500);
        var weak = BuildTeam("WEAK", 1200);
        var match = BuildFinishedMatch(home: weak, away: strong, homeGoals: 2, awayGoals: 0);

        _service.UpdateAfterMatch(match); // primera vez: aplica ranking, deja RankingApplied = true

        var weakRankingAfterFirst = weak.RankingFifa;
        var strongRankingAfterFirst = strong.RankingFifa;
        var logsAfterFirst = _auditRepository.GetAll().Count();

        _service.UpdateAfterMatch(match); // segunda vez: debe entrar al "return" y no hacer nada

        Assert.AreEqual(weakRankingAfterFirst, weak.RankingFifa);
        Assert.AreEqual(strongRankingAfterFirst, strong.RankingFifa);
        Assert.AreEqual(logsAfterFirst, _auditRepository.GetAll().Count());
    }
    
    
    private static Match BuildFinishedMatch(Team home, Team away, int homeGoals, int awayGoals)
    {
        var match = new Match(
            date: ValidDate,
            stadium: new Stadium("CENTENARIO", "MONTEVIDEO", "Historic stadium", 60000),
            homeTeam: home,
            awayTeam: away,
            group: 'A',
            phase: TournamentPhase.GroupStage);

        match.SetResult(homeGoals, awayGoals);
        match.Finish();
        return match;
    }

    private static Team BuildTeam(string name, int ranking) =>
        new(name, "CONMEBOL", ranking);
}