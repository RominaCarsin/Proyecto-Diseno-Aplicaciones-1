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
        IRepository<Group> repository = new FakeRepository<Group>();
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

    // ──────────────────────────────────────────────
    // RED: GetQualifiedTeams — casos de prueba
    // ──────────────────────────────────────────────

    [TestMethod]
    public void GetQualifiedTeams_With12FullGroups_ShouldReturn32Teams()
    {
        PopulateAllGroupsWithMinimumTeams();

        var qualifiedTeams = _service.GetQualifiedTeams().ToList();

        Assert.AreEqual(32, qualifiedTeams.Count,
            "Con 12 grupos deben clasificar exactamente 32 equipos.");
    }

    [TestMethod]
    public void GetQualifiedTeams_WithLessThan12Groups_ShouldThrowInvalidOperationException()
    {
        for (char label = 'A'; label < 'L'; label++)
        {
            _service.AddGroup(new GroupRequest { Label = label });
        }

        Assert.ThrowsException<InvalidOperationException>(() =>
            _service.GetQualifiedTeams(),
            "Deben existir exactamente 12 grupos para determinar clasificados.");
    }

    [TestMethod]
    public void GetQualifiedTeams_WhenGroupHasLessThan3Teams_ShouldThrowInvalidOperationException()
    {
        for (char label = 'A'; label <= 'L'; label++)
        {
            _service.AddGroup(new GroupRequest { Label = label });
            _service.AddTeamToGroup(label, new TeamRequest { Name = $"T-{label}-1", ConfederationName = "UEFA", RankingFifa = 1500 });
            _service.AddTeamToGroup(label, new TeamRequest { Name = $"T-{label}-2", ConfederationName = "CONMEBOL", RankingFifa = 1500 });
        }

        Assert.ThrowsException<InvalidOperationException>(() =>
            _service.GetQualifiedTeams(),
            "Cada grupo debe tener al menos 3 equipos para determinar 1ro, 2do y 3ro.");
    }

    [TestMethod]
    public void GetQualifiedTeams_ShouldPlaceBest4WinnersFirst()
    {
        SetupPredictableGroupResults();

        var qualifiedTeams = _service.GetQualifiedTeams().ToList();

        Assert.AreEqual("WINNER-01", qualifiedTeams[0].Name,
            "El mejor ganador debe estar en posicion 0.");
        Assert.AreEqual("WINNER-02", qualifiedTeams[1].Name,
            "El segundo mejor ganador debe estar en posicion 1.");
        Assert.AreEqual("WINNER-03", qualifiedTeams[2].Name,
            "El tercer mejor ganador debe estar en posicion 2.");
        Assert.AreEqual("WINNER-04", qualifiedTeams[3].Name,
            "El cuarto mejor ganador debe estar en posicion 3.");
    }

    [TestMethod]
    public void GetQualifiedTeams_ShouldPlaceWinnersBeforeRunnersUp()
    {
        SetupPredictableGroupResults();

        var qualifiedTeams = _service.GetQualifiedTeams().ToList();

        for (int i = 0; i < 12; i++)
        {
            Assert.IsTrue(qualifiedTeams[i].Name.Contains("WINNER"),
                $"Indice {i} debe ser un ganador de grupo.");
        }
        for (int i = 12; i < 24; i++)
        {
            Assert.IsTrue(qualifiedTeams[i].Name.Contains("RUNNERUP"),
                $"Indice {i} debe ser un segundo de grupo.");
        }
    }

    [TestMethod]
    public void GetQualifiedTeams_ShouldPlaceRunnersUpBeforeThirds()
    {
        SetupPredictableGroupResults();

        var qualifiedTeams = _service.GetQualifiedTeams().ToList();

        for (int i = 12; i < 24; i++)
        {
            Assert.IsTrue(qualifiedTeams[i].Name.Contains("RUNNERUP"),
                $"Indice {i} debe ser un segundo de grupo.");
        }
        for (int i = 24; i < 32; i++)
        {
            Assert.IsTrue(qualifiedTeams[i].Name.Contains("THIRD"),
                $"Indice {i} debe ser un tercero de grupo.");
        }
    }

    [TestMethod]
    public void GetQualifiedTeams_ShouldSelectOnly8BestThirds()
    {
        SetupPredictableGroupResults();

        var qualifiedTeams = _service.GetQualifiedTeams().ToList();

        Assert.AreEqual(8, qualifiedTeams.Count(t => t.Name.Contains("THIRD")),
            "Deben clasificar exactamente 8 terceros.");
        Assert.IsTrue(qualifiedTeams.Any(t => t.Name == "THIRD-BEST-A"),
            "El mejor tercero (A) debe estar en la lista.");
        Assert.IsTrue(qualifiedTeams.Any(t => t.Name == "THIRD-BEST-H"),
            "El 8vo mejor tercero (H) debe estar en la lista.");
        Assert.IsFalse(qualifiedTeams.Any(t => t.Name == "THIRD-WORST-I"),
            "El 9no mejor tercero (I) NO debe estar en la lista.");
        Assert.IsFalse(qualifiedTeams.Any(t => t.Name == "THIRD-WORST-L"),
            "El 12vo mejor tercero (L) NO debe estar en la lista.");
    }

    [TestMethod]
    public void GetQualifiedTeams_WhenTiedOnPoints_ShouldOrderByGoalDifference()
    {
        SetupTiedGroupsScenario();

        var qualifiedTeams = _service.GetQualifiedTeams().ToList();

        int highGdIndex = qualifiedTeams.FindIndex(t => t.Name == "HIGH-GD-TEAM");
        int lowGdIndex = qualifiedTeams.FindIndex(t => t.Name == "LOW-GD-TEAM");

        Assert.IsTrue(highGdIndex >= 0, "HIGH-GD-TEAM debe estar en la lista.");
        Assert.IsTrue(lowGdIndex >= 0, "LOW-GD-TEAM debe estar en la lista.");
        Assert.IsTrue(highGdIndex < lowGdIndex,
            "El equipo con mejor diferencia de gol debe aparecer antes.");
    }

    [TestMethod]
    public void GetQualifiedTeams_WhenTiedOnPointsAndGD_ShouldOrderByGoalsFor()
    {
        SetupTiedOnGDScenario();

        var qualifiedTeams = _service.GetQualifiedTeams().ToList();

        int highGfIndex = qualifiedTeams.FindIndex(t => t.Name == "HIGH-GF-TEAM");
        int lowGfIndex = qualifiedTeams.FindIndex(t => t.Name == "LOW-GF-TEAM");

        Assert.IsTrue(highGfIndex >= 0, "HIGH-GF-TEAM debe estar en la lista.");
        Assert.IsTrue(lowGfIndex >= 0, "LOW-GF-TEAM debe estar en la lista.");
        Assert.IsTrue(highGfIndex < lowGfIndex,
            "El equipo con mas goles a favor debe aparecer antes.");
    }

    [TestMethod]
    public void GetQualifiedTeams_ShouldReturnTeamsMappedFromDomain()
    {
        PopulateAllGroupsWithMinimumTeams();

        var qualifiedTeams = _service.GetQualifiedTeams().ToList();

        Assert.IsTrue(qualifiedTeams.All(t => !string.IsNullOrWhiteSpace(t.Name)),
            "Todo equipo debe tener nombre mapeado.");
        
        // CORRECCIÓN AQUÍ: Validamos que el mapeo no traiga nulos ni strings vacíos
        Assert.IsTrue(qualifiedTeams.All(t => !string.IsNullOrWhiteSpace(t.ConfederationName)),
            "Todo equipo debe tener confederacion mapeada.");
        
        Assert.IsTrue(qualifiedTeams.All(t => t.RankingFifa > 0),
            "Todo equipo debe tener ranking mapeado.");
    }

    // ──────────────────────────────────────────────
    // Helpers de prueba
    // ──────────────────────────────────────────────

    private void PopulateAllGroupsWithMinimumTeams()
    {
        for (char label = 'A'; label <= 'L'; label++)
        {
            _service.AddGroup(new GroupRequest { Label = label });
            AddFourTeams(label, $"TEAM-{label}-1", $"TEAM-{label}-2", $"TEAM-{label}-3", $"TEAM-{label}-4");
        }
    }

    private void SetupPredictableGroupResults()
    {
        AddGroupWithCustomResults('A', "WINNER-01", "RUNNERUP-A1", "THIRD-BEST-A", "OTHER-A",
            (6, 0), (7, 0), (6, 0), (2, 0), (6, 0), (2, 0));
        AddGroupWithCustomResults('B', "WINNER-02", "RUNNERUP-B2", "THIRD-BEST-B", "OTHER-B",
            (5, 0), (6, 0), (5, 0), (2, 0), (5, 0), (2, 0));
        AddGroupWithCustomResults('C', "WINNER-03", "RUNNERUP-C3", "THIRD-BEST-C", "OTHER-C",
            (4, 0), (5, 0), (4, 0), (2, 0), (4, 0), (2, 0));
        AddGroupWithCustomResults('D', "WINNER-04", "RUNNERUP-D4", "THIRD-BEST-D", "OTHER-D",
            (4, 0), (5, 0), (4, 0), (1, 0), (4, 0), (1, 0));
        AddGroupWithCustomResults('E', "WINNER-05", "RUNNERUP-E5", "THIRD-BEST-E", "OTHER-E",
            (3, 0), (4, 0), (3, 0), (2, 0), (3, 0), (2, 0));
        AddGroupWithCustomResults('F', "WINNER-06", "RUNNERUP-F6", "THIRD-BEST-F", "OTHER-F",
            (3, 0), (3, 0), (3, 0), (1, 0), (3, 0), (1, 0));
        AddGroupWithCustomResults('G', "WINNER-07", "RUNNERUP-G7", "THIRD-BEST-G", "OTHER-G",
            (2, 0), (3, 0), (2, 0), (2, 0), (2, 0), (2, 0));
        AddGroupWithCustomResults('H', "WINNER-08", "RUNNERUP-H8", "THIRD-BEST-H", "OTHER-H",
            (2, 0), (2, 0), (2, 0), (1, 0), (2, 0), (1, 0));
        AddGroupWithThirdNotQualifying('I', "WINNER-09", "RUNNERUP-I9", "THIRD-WORST-I", "OTHER-I");
        AddGroupWithThirdNotQualifying('J', "WINNER-10", "RUNNERUP-J10", "THIRD-WORST-J", "OTHER-J");
        AddGroupWithThirdNotQualifying('K', "WINNER-11", "RUNNERUP-K11", "THIRD-WORST-K", "OTHER-K");
        AddGroupWithThirdNotQualifying('L', "WINNER-12", "RUNNERUP-L12", "THIRD-WORST-L", "OTHER-L");
    }

    private void AddGroupWithCustomResults(char label,
        string winner, string runnerUp, string third, string other,
        (int h, int a) m1, (int h, int a) m2, (int h, int a) m3,
        (int h, int a) m4, (int h, int a) m5, (int h, int a) m6)
    {
        _service.AddGroup(new GroupRequest { Label = label });
        AddFourTeams(label, winner, runnerUp, third, other);

        AddMatchWithResult(label, winner, runnerUp, m1.h, m1.a);
        AddMatchWithResult(label, third, other, m2.h, m2.a);
        AddMatchWithResult(label, winner, third, m3.h, m3.a);
        AddMatchWithResult(label, runnerUp, other, m4.h, m4.a);
        AddMatchWithResult(label, winner, other, m5.h, m5.a);
        AddMatchWithResult(label, runnerUp, third, m6.h, m6.a);
    }

    private void AddGroupWithThirdNotQualifying(char label,
        string winner, string runnerUp, string third, string other)
    {
        _service.AddGroup(new GroupRequest { Label = label });
        AddFourTeams(label, winner, runnerUp, third, other);

        AddMatchWithResult(label, winner, runnerUp, 1, 0);
        AddMatchWithResult(label, other, third, 1, 0);
        AddMatchWithResult(label, winner, third, 1, 0);
        AddMatchWithResult(label, runnerUp, other, 1, 0);
        AddMatchWithResult(label, winner, other, 1, 0);
        AddMatchWithResult(label, runnerUp, third, 1, 0);
    }

    private void AddMatchWithResult(char group, string homeName, string awayName, int homeGoals, int awayGoals)
    {
        _service.AddMatchToGroup(group, new MatchRequest
        {
            Date = new DateTime(2026, 6, 1),
            Stadium = new StadiumRequest
            {
                Name = "Test Stadium",
                City = "Test City",
                Description = "Default test stadium",
                Capacity = 60000
            },
            HomeTeam = new TeamRequest { Name = homeName, ConfederationName = "UEFA", RankingFifa = 1500 },
            AwayTeam = new TeamRequest { Name = awayName, ConfederationName = "UEFA", RankingFifa = 1500 },
            Group = group,
            Phase = "GroupStage"
        });

        var groupResponse = _service.GetGroupByLabel(group);
        var matchId = groupResponse.Matches.Last().Id;
        _service.SetMatchResult(group, matchId, homeGoals, awayGoals);
    }

    private void SetupTiedGroupsScenario()
    {
        for (char label = 'A'; label <= 'L'; label++)
        {
            _service.AddGroup(new GroupRequest { Label = label });
            AddFourTeams(label,
                label == 'A' ? "HIGH-GD-TEAM" : label == 'B' ? "LOW-GD-TEAM" : $"WINNER-{label}",
                $"RU-{label}",
                $"THIRD-{label}",
                $"OTHER-{label}");
        }

        AddMatchWithResult('A', "HIGH-GD-TEAM", "RU-A", 4, 0);
        AddMatchWithResult('A', "THIRD-A", "OTHER-A", 1, 0);
        AddMatchWithResult('A', "HIGH-GD-TEAM", "THIRD-A", 3, 0);
        AddMatchWithResult('A', "RU-A", "OTHER-A", 1, 0);
        AddMatchWithResult('A', "HIGH-GD-TEAM", "OTHER-A", 3, 0);
        AddMatchWithResult('A', "RU-A", "THIRD-A", 1, 0);

        AddMatchWithResult('B', "LOW-GD-TEAM", "RU-B", 2, 0);
        AddMatchWithResult('B', "THIRD-B", "OTHER-B", 1, 0);
        AddMatchWithResult('B', "LOW-GD-TEAM", "THIRD-B", 2, 0);
        AddMatchWithResult('B', "RU-B", "OTHER-B", 1, 0);
        AddMatchWithResult('B', "LOW-GD-TEAM", "OTHER-B", 1, 0);
        AddMatchWithResult('B', "RU-B", "THIRD-B", 1, 0);
    }

    private void SetupTiedOnGDScenario()
    {
        for (char label = 'A'; label <= 'L'; label++)
        {
            _service.AddGroup(new GroupRequest { Label = label });
            AddFourTeams(label,
                label == 'A' ? "HIGH-GF-TEAM" : label == 'B' ? "LOW-GF-TEAM" : $"WINNER-{label}",
                $"RU-{label}",
                $"THIRD-{label}",
                $"OTHER-{label}");
        }

        AddMatchWithResult('A', "HIGH-GF-TEAM", "RU-A", 3, 0);
        AddMatchWithResult('A', "THIRD-A", "OTHER-A", 1, 0);
        AddMatchWithResult('A', "HIGH-GF-TEAM", "THIRD-A", 3, 0);
        AddMatchWithResult('A', "RU-A", "OTHER-A", 1, 0);
        AddMatchWithResult('A', "HIGH-GF-TEAM", "OTHER-A", 4, 0);
        AddMatchWithResult('A', "RU-A", "THIRD-A", 1, 0);

        AddMatchWithResult('B', "LOW-GF-TEAM", "RU-B", 2, 0);
        AddMatchWithResult('B', "THIRD-B", "OTHER-B", 1, 0);
        AddMatchWithResult('B', "LOW-GF-TEAM", "THIRD-B", 2, 0);
        AddMatchWithResult('B', "RU-B", "OTHER-B", 1, 0);
        AddMatchWithResult('B', "LOW-GF-TEAM", "OTHER-B", 1, 0);
        AddMatchWithResult('B', "RU-B", "THIRD-B", 1, 0);
    }

    private void AddFourTeams(char label, string t1, string t2, string t3, string t4)
    {
        var confs = new[] { "UEFA", "CONMEBOL", "CONCACAF", "AFC" };
        _service.AddTeamToGroup(label, new TeamRequest { Name = t1, ConfederationName = confs[0], RankingFifa = 1500 });
        _service.AddTeamToGroup(label, new TeamRequest { Name = t2, ConfederationName = confs[1], RankingFifa = 1500 });
        _service.AddTeamToGroup(label, new TeamRequest { Name = t3, ConfederationName = confs[2], RankingFifa = 1500 });
        _service.AddTeamToGroup(label, new TeamRequest { Name = t4, ConfederationName = confs[3], RankingFifa = 1500 });
    }

    private static TeamRequest CreateTeamRequest(string name, string confederation) => new()
    {
        Name = name,
        ConfederationName = confederation,
        RankingFifa = 1500
    };

    private static MatchRequest CreateValidMatchRequest(char group) => new()
    {
        Date = new DateTime(2026, 6, 1),
        Stadium = new StadiumRequest
        {
            Name = "Centenario",
            City = "Montevideo",
            Description = "Historic stadium",
            Capacity = 60000
        },
        HomeTeam = CreateTeamRequest("Uruguay","CONMEBOL"),
        AwayTeam = CreateTeamRequest("Argentina","CONMEBOL"),
        Group = group,
        Phase = "GroupStage"
    };
    
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