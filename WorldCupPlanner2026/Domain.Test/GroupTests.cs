namespace Domain.Test;

[TestClass]
public class GroupTests
{
    private const char ValidLabel = 'A';
    private const char LowercaseLabel = 'a';
    private const char InvalidLabel = 'Z';

    private Group CreateValidGroup() => new(ValidLabel);

    private Team CreateUruguayTeam() =>
        new("URUGUAY", "CONMEBOL", 1500);

    private Team CreateArgentinaTeam() =>
        new("ARGENTINA", "CONMEBOL", 1700);

    private Team CreateBrazilTeam() =>
        new("BRAZIL", "CONMEBOL", 1800);

    private Team CreateChileTeam() =>
        new("CHILE", "CONMEBOL", 1400);

    [TestMethod]
    public void CreateGroup_WithValidUppercaseLabel_ShouldCreateGroup()
    {
        var group = CreateValidGroup();

        Assert.AreEqual(ValidLabel, group.Label);
        Assert.AreEqual(0, group.Teams.Count);
    }

    [TestMethod]
    public void CreateGroup_WithLowercaseLabel_ShouldStoreUppercaseLabel()
    {
        var group = new Group(LowercaseLabel);

        Assert.AreEqual(ValidLabel, group.Label);
    }

    [TestMethod]
    public void UpdateLabel_WithLowercaseLabel_ShouldStoreUppercaseLabel()
    {
        var group = CreateValidGroup();

        group.UpdateLabel('b');

        Assert.AreEqual('B', group.Label);
    }

    [TestMethod]
    public void UpdateLabel_WithInvalidLabel_ShouldThrowArgumentException()
    {
        var group = CreateValidGroup();

        Assert.ThrowsException<ArgumentException>(() =>
            group.UpdateLabel(InvalidLabel));
    }

    [TestMethod]
    public void AddTeam_WithValidTeam_ShouldAddTeam()
    {
        var group = CreateValidGroup();
        var team = CreateUruguayTeam();

        group.AddTeam(team);

        Assert.AreEqual(1, group.Teams.Count);
        Assert.AreSame(team, group.Teams[0]);
    }

    [TestMethod]
    public void AddTeam_WithNullTeam_ShouldThrowArgumentException()
    {
        var group = CreateValidGroup();

        Assert.ThrowsException<ArgumentException>(() =>
            group.AddTeam(null));
    }

    [TestMethod]
    public void AddTeam_WithDuplicatedTeam_ShouldThrowArgumentException()
    {
        var group = CreateValidGroup();
        var team = CreateBrazilTeam();
        var duplicatedTeam = CreateBrazilTeam();

        group.AddTeam(team);

        Assert.ThrowsException<ArgumentException>(() =>
            group.AddTeam(duplicatedTeam));
    }

    [TestMethod]
    public void AddTeam_WhenGroupIsFull_ShouldThrowArgumentException()
    {
        var group = CreateValidGroup();

        AddFourTeams(group);

        var extraTeam = new Team("EXTRA", "OFC", 2000);

        Assert.ThrowsException<ArgumentException>(() =>
            group.AddTeam(extraTeam));
    }

    [TestMethod]
    public void RemoveTeam_WithExistingTeam_ShouldRemoveTeam()
    {
        var group = CreateValidGroup();
        var team = CreateArgentinaTeam();

        group.AddTeam(team);
        group.RemoveTeam(team);

        Assert.AreEqual(0, group.Teams.Count);
    }

    [TestMethod]
    public void RemoveTeam_WithNonExistingTeam_ShouldThrowArgumentException()
    {
        var group = CreateValidGroup();
        var team = CreateChileTeam();

        Assert.ThrowsException<ArgumentException>(() =>
            group.RemoveTeam(team));
    }

    [TestMethod]
    public void RemoveTeam_WithNullTeam_ShouldThrowArgumentException()
    {
        var group = CreateValidGroup();

        Assert.ThrowsException<ArgumentException>(() =>
            group.RemoveTeam(null));
    }

    [TestMethod]
    public void RemoveTeam_WithMultipleTeams_ShouldRemoveOnlySpecifiedTeam()
    {
        var group = CreateValidGroup();

        var argentina = CreateArgentinaTeam();
        var brazil = CreateBrazilTeam();

        group.AddTeam(argentina);
        group.AddTeam(brazil);

        group.RemoveTeam(argentina);

        Assert.AreEqual(1, group.Teams.Count);
        Assert.AreEqual(brazil, group.Teams[0]);
    }

    [TestMethod]
    public void GetTeamByPosition_WithValidPosition_ShouldReturnTeam()
    {
        var group = CreateValidGroup();

        var firstTeam = CreateArgentinaTeam();
        var secondTeam = CreateBrazilTeam();

        group.AddTeam(firstTeam);
        group.AddTeam(secondTeam);

        var result = group.GetTeamByPosition(1);

        Assert.AreEqual(firstTeam, result);
    }

    [TestMethod]
    public void GetTeamByPosition_WithInvalidPosition_ShouldThrowArgumentException()
    {
        var group = CreateValidGroup();

        Assert.ThrowsException<ArgumentException>(() =>
            group.GetTeamByPosition(0));

        Assert.ThrowsException<ArgumentException>(() =>
            group.GetTeamByPosition(5));
    }

    private static void AddFourTeams(Group group)
    {
        group.AddTeam(new Team("TEAM0", "AFC", 1000));
        group.AddTeam(new Team("TEAM1", "CAF", 1001));
        group.AddTeam(new Team("TEAM2", "CONCACAF", 1002));
        group.AddTeam(new Team("TEAM3", "UEFA", 1003));
    }
    
    [TestMethod]
    public void AddMatch_ShouldAddMatchToGroup()
    {
        Group group = new Group('A');
        Team uruguay = CreateUruguayTeam();
        Team argentina = CreateArgentinaTeam();

        group.AddTeam(uruguay);
        group.AddTeam(argentina);

        Match match = new Match(
            date: DateTime.Today,
            stadium: new Stadium("Centenario", " Montevideo", "ValidDescription", 20000), // o cualquier Stadium válido
            homeTeam: uruguay,
            awayTeam: argentina,
            group: group,                             // importante: el grupo debe ser este
            phase: TournamentPhase.GroupStage
        );

        group.AddMatch(match);

        Assert.AreEqual(1, group.Matches.Count);
    }
    
    [TestMethod]
    public void GetStandings_ShouldOrderByPoints()
    {
        Group group = new Group('A');
        Team uruguay = CreateUruguayTeam();
        Team argentina = CreateArgentinaTeam();

        group.AddTeam(uruguay);
        group.AddTeam(argentina);

        Match match = new Match(
            date: DateTime.Today,
            stadium: new Stadium("Centenario", " Montevideo", "ValidDescription", 20000), // o cualquier Stadium válido
            homeTeam: uruguay,
            awayTeam: argentina,
            group: group,                             // importante: el grupo debe ser este
            phase: TournamentPhase.GroupStage
        );
        match.SetResult(2, 0);

        group.AddMatch(match);

        var standings = group.GetStandings().ToList();

        Assert.AreEqual(uruguay, standings[0]);
    }
    
    [TestMethod]
    public void GetStandings_WhenSamePointsAndGoalDifference_ShouldOrderByGoalsFor()
    {
        Group group = new Group('A');

        Team uruguay = CreateUruguayTeam();
        Team argentina = CreateArgentinaTeam();
        Team brazil = CreateBrazilTeam();
        Team chile = CreateChileTeam();

        group.AddTeam(uruguay);
        group.AddTeam(argentina);
        group.AddTeam(brazil);
        group.AddTeam(chile);

        Match firstMatch = new Match(
            date: DateTime.Today,
            stadium: new Stadium("Centenario", " Montevideo", "ValidDescription", 20000), // o cualquier Stadium válido
            homeTeam: uruguay,
            awayTeam: argentina,
            group: group,                            
            phase: TournamentPhase.GroupStage
        );
        firstMatch.SetResult(2, 1);

        Match secondMatch = new Match(
            date: DateTime.Today,
            stadium: new Stadium("Centenario", " Montevideo", "ValidDescription", 20000), // o cualquier Stadium válido
            homeTeam: brazil,
            awayTeam: chile,
            group: group,                            
            phase: TournamentPhase.GroupStage
        );
        secondMatch.SetResult(3, 2);

        group.AddMatch(firstMatch);
        group.AddMatch(secondMatch);

        var standings = group.GetStandings().ToList();

        Assert.AreEqual(brazil, standings[0]);
    }
    
    
    [TestMethod]
    public void GetTeamByPosition_ShouldReturnTeamFromStandings()
    {
        Group group = new Group('A');

        Team uruguay = CreateUruguayTeam();
        Team argentina = CreateArgentinaTeam();

        group.AddTeam(uruguay);
        group.AddTeam(argentina);

        Match match = CreateGroupMatch(argentina, uruguay, group);
        match.SetResult(3, 0);

        group.AddMatch(match);

        Team team = group.GetTeamByPosition(1);

        Assert.AreEqual(argentina, team);
    }
}