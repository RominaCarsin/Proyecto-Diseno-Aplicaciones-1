namespace Domain.Test;


[TestClass]
public class GroupTests
{
   [TestMethod]
   public void CreateGroup_WithValidUppercaseLabel_ShouldCreateGroup()
   {
      Group group = new Group('A');

      Assert.AreEqual('A', group.Label);
      Assert.AreEqual(0, group.Teams.Count());
   }
   
   [TestMethod]
   public void CreateGroup_WithLowercaseLabel_ShouldStoreUppercase()
   {
      char lowerLabel = 'c';

      var group = new Group(lowerLabel);

      Assert.AreEqual('C', group.Label);
   }
   
   [TestMethod]
   public void AddTeam_WithValidTeam_ShouldAddTeam()
   {
      var group = new Group('A');
      var team = new Team("URUGUAY", "CONMEBOL", 1500);
      
      group.AddTeam(team);
      
      Assert.AreEqual(1, group.Teams.Count);
      Assert.AreSame(team, group.Teams[0]);
   }
   
   [TestMethod]
   public void AddTeam_WithNullTeam_ShouldThrowArgumentException()
   {
      var group = new Group('A');
      Assert.ThrowsException<ArgumentException>(() => group.AddTeam(null));
   }
   
   [TestMethod]
   public void AddTeam_WithDuplicatedTeam_ShouldThrowArgumentException()
   {
      var group = new Group('A');
      var team1 = new Team("BRAZIL", "CONMEBOL", 1800);
      var team2 = new Team("BRAZIL", "CONMEBOL", 1800);
      group.AddTeam(team1);
      Assert.ThrowsException<ArgumentException>(() => group.AddTeam(team2));
   }
   
   [TestMethod]
   public void AddTeam_WhenGroupIsFull_ShouldThrowArgumentException()
   {
      var group = new Group('A');
      for (int i = 0; i < 4; i++)
         group.AddTeam(new Team($"Team{i}", "AFC", 1000 + i));

      var extraTeam = new Team("Extra", "OFC", 2000);
      Assert.ThrowsException<ArgumentException>(() => group.AddTeam(extraTeam));
   }
   
   [TestMethod]
   public void RemoveTeam_WithExistingTeam_ShouldRemoveTeam()
   {
      var group = new Group('A');
      var team = new Team("ARGENTINA", "CONMEBOL", 1700);
      group.AddTeam(team);
      group.RemoveTeam(team);
      Assert.AreEqual(0, group.Teams.Count);
   }
   
   [TestMethod]
   public void RemoveTeam_WithNonExistingTeam_ShouldThrowArgumentException()
   {
      var group = new Group('A');
      var team = new Team("CHILE", "CONMEBOL", 1400);
      Assert.ThrowsException<ArgumentException>(() => group.RemoveTeam(team));
   }
   
   [TestMethod]
   public void RemoveTeam_WithMultipleTeams_ShouldRemoveOnlySpecifiedTeam()
   {
      var group = new Group('A');

      var ARGENTINA = new Team("ARGENTINA", "CONMEBOL", 1700);
      var BRAZIL = new Team("BRAZIL", "CONMEBOL", 1800);

      group.AddTeam(ARGENTINA);
      group.AddTeam(BRAZIL);

      group.RemoveTeam(ARGENTINA);

      Assert.AreEqual(1, group.Teams.Count);
      Assert.AreEqual("BRAZIL", group.Teams[0].Name);
   }
   
   [TestMethod]
   public void GetTeamByPosition_WithValidPosition_ShouldReturnTeam()
   {
      var group = new Group('A');
      var team1 = new Team("Team1", "UEFA", 1000);
      var team2 = new Team("Team2", "UEFA", 1100);
      group.AddTeam(team1);
      group.AddTeam(team2);

      var result = group.GetTeamByPosition(1);
      Assert.AreEqual(team1, result);
   }
   
   [TestMethod]
   public void GetTeamByPosition_WithInvalidPosition_ShouldThrowArgumentException()
   {
      var group = new Group('A');
      Assert.ThrowsException<ArgumentException>(() => group.GetTeamByPosition(0));
      Assert.ThrowsException<ArgumentException>(() => group.GetTeamByPosition(5));
   }
   
   [TestMethod]
   public void CreateGroup_WithLowercaseLabel_ShouldStoreUppercaseLabel()
   {
      var group = new Group('a');

      Assert.AreEqual('A', group.Label);
   }
   
   [TestMethod]
   public void UpdateLabel_WithLowercaseLabel_ShouldStoreUppercaseLabel()
   {
      var group = new Group('A');

      group.UpdateLabel('b');

      Assert.AreEqual('B', group.Label);
   }
   
   [TestMethod]
   public void RemoveTeam_WithNullTeam_ShouldThrowArgumentException()
   {
      var group = new Group('A');

      Assert.ThrowsException<ArgumentException>(() =>
         group.RemoveTeam(null));
   }
   
   [TestMethod]
   public void UpdateLabel_WithInvalidLabel_ShouldThrowArgumentException()
   {
      var group = new Group('A');

      Assert.ThrowsException<ArgumentException>(() =>
         group.UpdateLabel('Z'));
   }
}