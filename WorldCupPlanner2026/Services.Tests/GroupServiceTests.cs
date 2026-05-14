using Domain;
using DTOs;
using IRepository;
using IServices;
using Repository;
using Services;

namespace Services.Tests;

[TestClass]
public class GroupServiceTests
{
    private const char ValidGroupLabel = 'A';
    private const int ValidRanking = 1500;

    private static readonly DateTime ValidMatchDate = new(2026, 6, 1);

    private IGroupService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        IRepository<Group> repository = new Repository<Group>();
        _service = new GroupService(repository);
    }
    
    [TestMethod]
    public void AddGroup_WithValidRequest_ShouldPersistGroupWithExpectedLabel()
    {
        _service.AddGroup(BuildGroupRequest(ValidGroupLabel));

        var groups = _service.GetGroups().ToList();

        Assert.AreEqual(1, groups.Count);
        Assert.AreEqual(ValidGroupLabel, groups[0].Label);
    }
    
    [TestMethod]
    public void GetGroupByLabel_WithExistingLabel_ShouldReturnCorrectGroup()
    {
        _service.AddGroup(BuildGroupRequest(ValidGroupLabel));

        var group = _service.GetGroupByLabel(ValidGroupLabel);

        Assert.AreEqual(ValidGroupLabel, group.Label);
    }
    
    [TestMethod]
    public void DeleteGroup_WithExistingGroup_ShouldRemoveGroupFromRepository()
    {
        _service.AddGroup(BuildGroupRequest(ValidGroupLabel));

        _service.DeleteGroup(ValidGroupLabel);

        Assert.AreEqual(0, _service.GetGroups().Count());
    }
    
    [TestMethod]
    public void AddTeamToGroup_WithValidTeam_ShouldAddTeamToGroup()
    {
        _service.AddGroup(BuildGroupRequest(ValidGroupLabel));

        _service.AddTeamToGroup(ValidGroupLabel, BuildTeamRequest("URUGUAY", "CONMEBOL"));

        var group = _service.GetGroupByLabel(ValidGroupLabel);
        Assert.AreEqual(1, group.Teams.Count);
    }
    
    [TestMethod]
    public void RemoveTeamFromGroup_WithExistingTeam_ShouldRemoveTeamFromGroup()
    {
        _service.AddGroup(BuildGroupRequest(ValidGroupLabel));
        _service.AddTeamToGroup(ValidGroupLabel, BuildTeamRequest("URUGUAY", "CONMEBOL"));

        _service.RemoveTeamFromGroup(ValidGroupLabel, BuildTeamRequest("URUGUAY", "CONMEBOL"));

        var group = _service.GetGroupByLabel(ValidGroupLabel);
        Assert.AreEqual(0, group.Teams.Count);
    }
    
    [TestMethod]
    public void AddMatchToGroup_WithValidMatch_ShouldAddMatchToGroup()
    {
        AddGroupWithUruguayAndArgentina();

        _service.AddMatchToGroup(ValidGroupLabel, BuildValidMatchRequest());

        var group = _service.GetGroupByLabel(ValidGroupLabel);
        Assert.AreEqual(1, group.Matches.Count);
    }
    
    [TestMethod]
    public void SetMatchResult_WithValidResult_ShouldUpdateTeamStandings()
    {
        AddGroupWithUruguayAndArgentina();
        _service.AddMatchToGroup(ValidGroupLabel, BuildValidMatchRequest());

        var matchId = _service.GetGroupByLabel(ValidGroupLabel).Matches[0].Id;
        _service.SetMatchResult(ValidGroupLabel, matchId, homeGoals: 3, awayGoals: 1);

        var standings = _service.GetGroupStandings(ValidGroupLabel).ToList();
        Assert.AreEqual("URUGUAY", standings[0].Name);
        Assert.AreEqual(3, standings[0].Points);
    }
    
    [TestMethod]
    public void GetTeamByPosition_WithValidPosition_ShouldReturnTeamInThatPosition()
    {
        AddGroupWithUruguayAndArgentina();
        _service.AddMatchToGroup(ValidGroupLabel, BuildValidMatchRequest());

        var matchId = _service.GetGroupByLabel(ValidGroupLabel).Matches[0].Id;
        _service.SetMatchResult(ValidGroupLabel, matchId, homeGoals: 3, awayGoals: 0);

        var team = _service.GetTeamByPosition(ValidGroupLabel, position: 1);

        Assert.AreEqual("URUGUAY", team.Name);
    }
    
    [TestMethod]
    public void FinishMatch_WithValidMatch_ShouldSetMatchAsFinishedAndLocked()
    {
        AddGroupWithUruguayAndArgentina();
        _service.AddMatchToGroup(ValidGroupLabel, BuildValidMatchRequest());

        var matchId = _service.GetGroupByLabel(ValidGroupLabel).Matches[0].Id;
        _service.FinishMatch(ValidGroupLabel, matchId);

        var group = _service.GetGroupByLabel(ValidGroupLabel);
        Assert.AreEqual(MatchStatus.Finished.ToString(), group.Matches[0].Status);
        Assert.IsTrue(group.Matches[0].IsLocked);
    }

    [TestMethod]
    public void FinishMatch_WithNonExistingGroup_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _service.FinishMatch('Z', matchId: 1));
    }

    [TestMethod]
    public void FinishMatch_WithNonExistingMatch_ShouldThrowArgumentException()
    {
        _service.AddGroup(BuildGroupRequest(ValidGroupLabel));

        Assert.ThrowsException<ArgumentException>(() =>
            _service.FinishMatch(ValidGroupLabel, matchId: 999));
    }
    
    private void AddGroupWithUruguayAndArgentina()
    {
        _service.AddGroup(BuildGroupRequest(ValidGroupLabel));
        _service.AddTeamToGroup(ValidGroupLabel, BuildTeamRequest("URUGUAY", "CONMEBOL"));
        _service.AddTeamToGroup(ValidGroupLabel, BuildTeamRequest("ARGENTINA", "UEFA"));
    }

    private static GroupRequest BuildGroupRequest(char label) =>
        new GroupRequest { Label = label };

    private static TeamRequest BuildTeamRequest(string name, string confederation) =>
        new TeamRequest
        {
            Name = name,
            ConfederationName = confederation,
            RankingFifa = ValidRanking
        };

    private static MatchRequest BuildValidMatchRequest() =>
        new MatchRequest
        {
            Date = ValidMatchDate,
            Stadium = new StadiumRequest
            {
                Name = "CENTENARIO",
                City = "MONTEVIDEO",
                Description = "Historic stadium",
                Capacity = 60000
            },
            HomeTeam = BuildTeamRequest("URUGUAY", "CONMEBOL"),
            AwayTeam = BuildTeamRequest("ARGENTINA", "UEFA"),
            Group = ValidGroupLabel,
            Phase = "GroupStage"
        };
}