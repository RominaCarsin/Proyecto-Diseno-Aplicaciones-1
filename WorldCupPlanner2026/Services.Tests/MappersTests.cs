using Domain;
using DTOs;
using Services.Mappers;

namespace Services.Tests;

[TestClass]
public class MappersTests
{
    [TestMethod]
    public void GroupMapper_ToEntity_ShouldCreateGroupWithGivenLabel()
    {
        var request = new GroupRequest { Label = 'A' };

        var result = GroupMapper.ToEntity(request);

        Assert.AreEqual('A', result.Label);
    }

    [TestMethod]
    public void GroupMapper_FromEntity_ShouldMapAllProperties()
    {
        var group = new Group('B');
        group.AddTeam(new Team("TEST_TEAM", "UEFA", 1500));

        var result = GroupMapper.FromEntity(group);

        Assert.AreEqual('B', result.Label);
        Assert.AreEqual(1, result.Teams.Count);
        Assert.AreEqual("TEST_TEAM", result.Teams.First().Name);
    }

    [TestMethod]
    public void MatchMapper_ToEntity_WithValidPhaseString_ShouldResolvePhaseCorrectly()
    {
        var request = new MatchRequest
        {
            Date = new DateTime(2026, 7, 1),
            Stadium = new StadiumRequest { Name = "S", City = "C", Description = "D", Capacity = 50000 },
            HomeTeam = new TeamRequest { Name = "HOME", ConfederationName = "UEFA", RankingFifa = 1500 },
            AwayTeam = new TeamRequest { Name = "AWAY", ConfederationName = "CONMEBOL", RankingFifa = 1400 },
            Phase = "QuarterFinals"
        };

        var result = MatchMapper.ToEntity(request);

        Assert.AreEqual(TournamentPhase.QuarterFinals, result.Phase);
    }

    [TestMethod]
    public void MatchMapper_ToEntity_WithNullGroup_ShouldSetGroupNull()
    {
        var request = new MatchRequest
        {
            Date = new DateTime(2026, 7, 1),
            Stadium = new StadiumRequest { Name = "S", City = "C", Description = "D", Capacity = 50000 },
            HomeTeam = new TeamRequest { Name = "HOME", ConfederationName = "UEFA", RankingFifa = 1500 },
            AwayTeam = new TeamRequest { Name = "AWAY", ConfederationName = "CONMEBOL", RankingFifa = 1400 },
            Group = null
        };

        var result = MatchMapper.ToEntity(request);

        Assert.IsNull(result.Group);
    }

    [TestMethod]
    public void MatchMapper_ToResponse_ShouldMapAllPropertiesWithDefaultIsLocked()
    {
        var stadium = new Stadium("STADIUM", "CITY", "Desc", 50000);
        var home = new Team("HOME", "UEFA", 1500);
        var away = new Team("AWAY", "CONMEBOL", 1400);
        var match = new Match(
            new DateTime(2026, 6, 1), stadium, home, away,
            group: 'A', phase: TournamentPhase.GroupStage, matchCode: "G1");

        var result = MatchMapper.ToResponse(match);

        Assert.AreEqual(match.MatchId, result.Id);
        Assert.AreEqual("A", result.Group.ToString());
        Assert.AreEqual("GroupStage", result.Phase);
        Assert.AreEqual("G1", result.MatchCode);
        Assert.IsFalse(result.IsLocked);
        Assert.IsFalse(result.IsFinished);
        Assert.IsFalse(result.HasResult);
        Assert.IsNull(result.Winner);
        Assert.IsNull(result.HomeGoals);
        Assert.IsNull(result.AwayGoals);
        Assert.AreEqual(0, result.HomeYellowCards);
        Assert.AreEqual(0, result.AwayYellowCards);
    }

    [TestMethod]
    public void MatchMapper_ToResponse_WithResultAndIsLocked_ShouldMapCorrectly()
    {
        var stadium = new Stadium("STADIUM", "CITY", "Desc", 50000);
        var home = new Team("HOME", "UEFA", 1500);
        var away = new Team("AWAY", "CONMEBOL", 1400);
        var match = new Match(
            new DateTime(2026, 6, 1), stadium, home, away,
            group: 'A', phase: TournamentPhase.GroupStage, matchCode: "G1");

        match.SetResult(2, 1);

        var result = MatchMapper.ToResponse(match, isLocked: true);

        Assert.IsTrue(result.IsLocked);
        Assert.IsTrue(result.HasResult);
        Assert.AreEqual(2, result.HomeGoals);
        Assert.AreEqual(1, result.AwayGoals);
        Assert.AreEqual("HOME", result.Winner!.Name);
    }

    [TestMethod]
    public void MatchMapper_ToResponse_WithNullPhase_ShouldDefaultToGroupStage()
    {
        var request = new MatchRequest
        {
            Date = new DateTime(2026, 7, 1),
            Stadium = new StadiumRequest { Name = "S", City = "C", Description = "D", Capacity = 50000 },
            HomeTeam = new TeamRequest { Name = "HOME", ConfederationName = "UEFA", RankingFifa = 1500 },
            AwayTeam = new TeamRequest { Name = "AWAY", ConfederationName = "CONMEBOL", RankingFifa = 1400 },
            Phase = ""
        };

        var result = MatchMapper.ToEntity(request);

        Assert.AreEqual(TournamentPhase.GroupStage, result.Phase);
    }

    [TestMethod]
    public void MatchMapper_ToEntity_WithInvalidPhase_ShouldThrowArgumentException()
    {
        var request = new MatchRequest
        {
            Date = new DateTime(2026, 7, 1),
            Stadium = new StadiumRequest { Name = "S", City = "C", Description = "D", Capacity = 50000 },
            HomeTeam = new TeamRequest { Name = "HOME", ConfederationName = "UEFA", RankingFifa = 1500 },
            AwayTeam = new TeamRequest { Name = "AWAY", ConfederationName = "CONMEBOL", RankingFifa = 1400 },
            Phase = "Gibberish"
        };

        Assert.ThrowsException<ArgumentException>(() =>
            MatchMapper.ToEntity(request));
    }
}
