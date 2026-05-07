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
      var team = new Team("Uruguay", "CONMEBOL", 1500);
      
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
      var team1 = new Team("Brasil", "CONMEBOL", 1800);
      var team2 = new Team("BRASIL", "CONMEBOL", 1800);
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
      var team = new Team("Argentina", "CONMEBOL", 1700);
      group.AddTeam(team);
      group.RemoveTeam(team);
      Assert.AreEqual(0, group.Teams.Count);
   }
   
   [TestMethod]
   public void RemoveTeam_WithNonExistingTeam_ShouldThrowArgumentException()
   {
      var group = new Group('A');
      var team = new Team("Chile", "CONMEBOL", 1400);
      Assert.ThrowsException<ArgumentException>(() => group.RemoveTeam(team));
   }
}