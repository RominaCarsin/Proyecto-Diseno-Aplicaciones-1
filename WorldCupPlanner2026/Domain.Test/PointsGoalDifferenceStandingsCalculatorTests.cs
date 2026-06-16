namespace Domain.Test;

[TestClass]
public class PointsGoalDifferenceStandingsCalculatorTests
{
    [TestMethod]
    public void Order_WithNoStats_ShouldReturnAllTeamsWithZeroPoints()
    {
        var uruguay = TeamTestData.CreateUruguayTeam();
        var argentina = TeamTestData.CreateArgentinaTeam();
        var calculator = new GroupOrderPointsGoalDiffCalculator();
        var teams = new List<Team> { uruguay, argentina };

        var result = calculator.Order(teams).ToList();

        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.All(t => t.Points == 0));
    }

    [TestMethod]
    public void Order_WithDifferentPoints_ShouldOrderByPointsDescending()
    {
        var uruguay = TeamTestData.CreateUruguayTeam();
        var argentina = TeamTestData.CreateArgentinaTeam();
        uruguay.RecordWin(2, 0);
        argentina.RecordLoss(0, 2);
        var calculator = new GroupOrderPointsGoalDiffCalculator();
        var teams = new List<Team> { uruguay, argentina };

        var result = calculator.Order(teams).ToList();

        Assert.AreEqual(uruguay, result[0]);
    }

    [TestMethod]
    public void Order_WithSamePoints_ShouldOrderByGoalDifference()
    {
        var uruguay = TeamTestData.CreateUruguayTeam();
        var argentina = TeamTestData.CreateArgentinaTeam();
        var brazil = TeamTestData.CreateBrazilTeam();
        var chile = TeamTestData.CreateChileTeam();
        uruguay.RecordWin(1, 0);
        chile.RecordLoss(0, 1);
        argentina.RecordWin(3, 0);
        brazil.RecordLoss(0, 3);
        var calculator = new GroupOrderPointsGoalDiffCalculator();
        var teams = new List<Team> { uruguay, argentina, brazil, chile };

        var result = calculator.Order(teams).ToList();

        Assert.AreEqual(argentina, result[0]);
        Assert.AreEqual(uruguay, result[1]);
    }

    [TestMethod]
    public void Order_WithSamePointsAndGoalDifference_ShouldOrderByGoalsFor()
    {
        var uruguay = TeamTestData.CreateUruguayTeam();
        var argentina = TeamTestData.CreateArgentinaTeam();
        var brazil = TeamTestData.CreateBrazilTeam();
        var chile = TeamTestData.CreateChileTeam();
        uruguay.RecordWin(2, 1);
        argentina.RecordLoss(1, 2);
        brazil.RecordWin(3, 2);
        chile.RecordLoss(2, 3);
        var calculator = new GroupOrderPointsGoalDiffCalculator();
        var teams = new List<Team> { uruguay, argentina, brazil, chile };

        var result = calculator.Order(teams).ToList();

        Assert.AreEqual(brazil, result[0]);
        Assert.AreEqual(uruguay, result[1]);
    }

    [TestMethod]
    public void Order_WithMultipleMatches_ShouldReflectAccumulatedStats()
    {
        var uruguay = TeamTestData.CreateUruguayTeam();
        var argentina = TeamTestData.CreateArgentinaTeam();
        var brazil = TeamTestData.CreateBrazilTeam();
        var chile = TeamTestData.CreateChileTeam();
        uruguay.RecordWin(1, 0);
        uruguay.RecordWin(2, 0);
        argentina.RecordLoss(0, 1);
        brazil.RecordLoss(0, 2);
        var calculator = new GroupOrderPointsGoalDiffCalculator();
        var teams = new List<Team> { uruguay, argentina, brazil, chile };

        var result = calculator.Order(teams).ToList();

        Assert.AreEqual(uruguay, result[0]);
        Assert.AreEqual(6, uruguay.Points);
    }
}
