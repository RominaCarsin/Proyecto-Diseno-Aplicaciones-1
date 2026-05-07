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
}