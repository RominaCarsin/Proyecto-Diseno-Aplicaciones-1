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
    public void GetStandings_ShouldReturnTeamsOrderedByPointsGoalDifferenceGoalsInFavorAndName()
    {
        var group = CreateValidGroup();

        var argentina = CreateArgentinaTeam();
        var brazil = CreateBrazilTeam();
        var chile = CreateChileTeam();
        var uruguay = CreateUruguayTeam();

        argentina.AddMatchResult(3, 1); // 3 pts, GD +2, GF 3
        brazil.AddMatchResult(2, 0);    // 3 pts, GD +2, GF 2
        chile.AddMatchResult(1, 1);     // 1 pt, GD 0, GF 1
        uruguay.AddMatchResult(0, 2);   // 0 pts, GD -2, GF 0

        group.AddTeam(chile);
        group.AddTeam(uruguay);
        group.AddTeam(brazil);
        group.AddTeam(argentina);

        var standings = group.GetStandings().ToList();

        Assert.AreEqual(argentina, standings[0]);
        Assert.AreEqual(brazil, standings[1]);
        Assert.AreEqual(chile, standings[2]);
        Assert.AreEqual(uruguay, standings[3]);
    }
}