namespace Domain.Test;

[TestClass]
public class GroupStageSimulatorTests
{
    private const char LabelA = 'A';
    private const char LabelB = 'B';
    private const int Seed = 42;

    private Stadium CreateStadium() =>
        new("Centenario", "Montevideo", "ValidDescription", 20000);

    private Team CreateTeam(string name, string confederation, int ranking) =>
        new(name, confederation, ranking);

    private Match CreateMatch(Team home, Team away) =>
        new(date: DateTime.Today, stadium: CreateStadium(), homeTeam: home, awayTeam: away);

    private Group CreateGroupWithMatch(char label, Team home, Team away, out Match match)
    {
        var group = new Group(label);
        group.AddTeam(home);
        group.AddTeam(away);

        match = CreateMatch(home, away);
        group.AddMatch(match);

        return group;
    }

    [TestMethod]
    public void Simulate_WithPendingMatch_ShouldSetMatchResult()
    {
        var home = CreateTeam("HOME", "CONMEBOL", 1500);
        var away = CreateTeam("AWAY", "UEFA", 1700);
        var group = CreateGroupWithMatch(LabelA, home, away, out var match);

        new GroupStageSimulator(Seed).Simulate(new[] { group });

        Assert.IsTrue(match.HasResult);
        Assert.IsNotNull(match.HomeGoals);
        Assert.IsNotNull(match.AwayGoals);
    }

    [TestMethod]
    public void Simulate_WithPendingMatch_ShouldUpdateTeamStats()
    {
        var home = CreateTeam("HOME", "CONMEBOL", 1500);
        var away = CreateTeam("AWAY", "UEFA", 1700);
        var group = CreateGroupWithMatch(LabelA, home, away, out var match);

        new GroupStageSimulator(Seed).Simulate(new[] { group });

        int totalPoints = home.Points + away.Points;
        Assert.IsTrue(totalPoints == 2 || totalPoints == 3);
    }

    [TestMethod]
    public void Simulate_WithMultipleMatches_ShouldSetResultForAllMatches()
    {
        var home1 = CreateTeam("HOME1", "CONMEBOL", 1500);
        var away1 = CreateTeam("AWAY1", "UEFA", 1700);
        var home2 = CreateTeam("HOME2", "AFC", 1800);
        var away2 = CreateTeam("AWAY2", "CAF", 1400);

        var group = new Group(LabelA);
        group.AddTeam(home1);
        group.AddTeam(away1);
        group.AddTeam(home2);
        group.AddTeam(away2);

        var match1 = CreateMatch(home1, away1);
        var match2 = CreateMatch(home2, away2);
        group.AddMatch(match1);
        group.AddMatch(match2);

        new GroupStageSimulator(Seed).Simulate(new[] { group });

        Assert.IsTrue(match1.HasResult);
        Assert.IsTrue(match2.HasResult);
    }

    [TestMethod]
    public void Simulate_WithEqualRankings_ShouldGenerateDraw()
    {
        var home = CreateTeam("HOME", "CONMEBOL", 1500);
        var away = CreateTeam("AWAY", "UEFA", 1500);
        var group = CreateGroupWithMatch(LabelA, home, away, out var match);

        new GroupStageSimulator(Seed).Simulate(new[] { group });

        Assert.AreEqual(match.HomeGoals, match.AwayGoals);
    }

    [TestMethod]
    public void Simulate_WithEqualRankings_ShouldNotProduceAWinner()
    {
        var home = CreateTeam("HOME", "CONMEBOL", 1500);
        var away = CreateTeam("AWAY", "UEFA", 1500);
        var group = CreateGroupWithMatch(LabelA, home, away, out var match);

        new GroupStageSimulator(Seed).Simulate(new[] { group });

        Assert.IsNull(match.Winner);
    }

    [TestMethod]
    public void Simulate_WithEqualRankings_ShouldGiveBothTeamsOnePoint()
    {
        var home = CreateTeam("HOME", "CONMEBOL", 1500);
        var away = CreateTeam("AWAY", "UEFA", 1500);
        var group = CreateGroupWithMatch(LabelA, home, away, out var match);

        new GroupStageSimulator(Seed).Simulate(new[] { group });

        Assert.AreEqual(1, home.Points);
        Assert.AreEqual(1, away.Points);
        Assert.AreEqual(0, home.GoalDifference);
        Assert.AreEqual(0, away.GoalDifference);
    }

    [TestMethod]
    public void Simulate_WithSameSeedAndSameLabel_ShouldGenerateSameResult()
    {
        var home1 = CreateTeam("HOME", "CONMEBOL", 2500);
        var away1 = CreateTeam("AWAY", "UEFA", 300);
        var group1 = CreateGroupWithMatch(LabelA, home1, away1, out var match1);

        var home2 = CreateTeam("HOME", "CONMEBOL", 2500);
        var away2 = CreateTeam("AWAY", "UEFA", 300);
        var group2 = CreateGroupWithMatch(LabelA, home2, away2, out var match2);

        new GroupStageSimulator(Seed).Simulate(new[] { group1 });
        new GroupStageSimulator(Seed).Simulate(new[] { group2 });

        Assert.AreEqual(match1.HomeGoals, match2.HomeGoals);
        Assert.AreEqual(match1.AwayGoals, match2.AwayGoals);
    }

    [TestMethod]
    public void Simulate_ShouldGenerateGoalsWithinValidRange()
    {
        var home = CreateTeam("HOME", "CONMEBOL", 2500);
        var away = CreateTeam("AWAY", "UEFA", 300);
        var group = CreateGroupWithMatch(LabelA, home, away, out var match);

        new GroupStageSimulator(Seed).Simulate(new[] { group });

        Assert.IsTrue(match.HomeGoals is >= 0 and <= 4);
        Assert.IsTrue(match.AwayGoals is >= 0 and <= 4);
    }

    [TestMethod]
    public void Simulate_WithMultipleGroups_ShouldSimulateEachGroup()
    {
        var homeA = CreateTeam("HOMEA", "CONMEBOL", 1500);
        var awayA = CreateTeam("AWAYA", "UEFA", 1700);
        var groupA = CreateGroupWithMatch(LabelA, homeA, awayA, out var matchA);

        var homeB = CreateTeam("HOMEB", "AFC", 1800);
        var awayB = CreateTeam("AWAYB", "CAF", 1400);
        var groupB = CreateGroupWithMatch(LabelB, homeB, awayB, out var matchB);

        new GroupStageSimulator(Seed).Simulate(new[] { groupA, groupB });

        Assert.IsTrue(matchA.HasResult);
        Assert.IsTrue(matchB.HasResult);
    }
    
    [TestMethod]
    public void Simulate_WithCustomEngine_ShouldUseEngineResult()
    {
        var home = CreateTeam("HOME", "CONMEBOL", 1500);
        var away = CreateTeam("AWAY", "UEFA", 1700);
        var group = CreateGroupWithMatch(LabelA, home, away, out var match);

        var engine = new FixedSimulationEngine(homeGoals: 4, awayGoals: 1);

        new GroupStageSimulator(Seed, engine).Simulate(new[] { group });

        Assert.AreEqual(4, match.HomeGoals);
        Assert.AreEqual(1, match.AwayGoals);
    }

    [TestMethod]
    public void Simulate_WithCustomEngine_ShouldRegisterCardsFromSimulationResult()
    {
        var home = CreateTeam("HOME", "CONMEBOL", 1500);
        var away = CreateTeam("AWAY", "UEFA", 1700);
        var group = CreateGroupWithMatch(LabelA, home, away, out var match);

        var engine = new FixedSimulationEngineWithCards(
            homeGoals: 2,
            awayGoals: 1,
            homeYellow: 3,
            awayYellow: 2,
            homeRed: 1,
            awayRed: 0);

        new GroupStageSimulator(Seed, engine).Simulate(new[] { group });

        Assert.AreEqual(2, match.HomeGoals);
        Assert.AreEqual(1, match.AwayGoals);
        Assert.AreEqual(3, match.YellowCardsOf(home));
        Assert.AreEqual(2, match.YellowCardsOf(away));
        Assert.AreEqual(1, match.RedCardsOf(home));
        Assert.AreEqual(0, match.RedCardsOf(away));
    }

    [TestMethod]
    public void Simulate_ShouldGenerateCardsWithinValidRange()
    {
        var home = CreateTeam("HOME", "CONMEBOL", 2500);
        var away = CreateTeam("AWAY", "UEFA", 300);
        var group = CreateGroupWithMatch(LabelA, home, away, out var match);

        new GroupStageSimulator(Seed).Simulate(new[] { group });

        Assert.IsTrue(match.YellowCardsOf(home) >= 0 && match.YellowCardsOf(home) <= 5);
        Assert.IsTrue(match.YellowCardsOf(away) >= 0 && match.YellowCardsOf(away) <= 5);
        Assert.IsTrue(match.RedCardsOf(home) >= 0 && match.RedCardsOf(home) <= 1);
        Assert.IsTrue(match.RedCardsOf(away) >= 0 && match.RedCardsOf(away) <= 1);
    }
    
    private sealed class FixedSimulationEngine : ISimulationEngine
    {
        private readonly int _homeGoals;
        private readonly int _awayGoals;

        public FixedSimulationEngine(int homeGoals, int awayGoals)
        {
            _homeGoals = homeGoals;
            _awayGoals = awayGoals;
        }

        public string Name => "Fixed";

        public SimulationResult Simulate(
            Team homeTeam,
            Team awayTeam,
            TournamentPhase phase,
            Random random)
        {
            return new SimulationResult(_homeGoals, _awayGoals);
        }
    }

    private sealed class FixedSimulationEngineWithCards : ISimulationEngine
    {
        private readonly int _homeGoals;
        private readonly int _awayGoals;
        private readonly int _homeYellowCards;
        private readonly int _awayYellowCards;
        private readonly int _homeRedCards;
        private readonly int _awayRedCards;

        public FixedSimulationEngineWithCards(
            int homeGoals,
            int awayGoals,
            int homeYellow,
            int awayYellow,
            int homeRed,
            int awayRed)
        {
            _homeGoals = homeGoals;
            _awayGoals = awayGoals;
            _homeYellowCards = homeYellow;
            _awayYellowCards = awayYellow;
            _homeRedCards = homeRed;
            _awayRedCards = awayRed;
        }

        public string Name => "FixedWithCards";

        public SimulationResult Simulate(
            Team homeTeam,
            Team awayTeam,
            TournamentPhase phase,
            Random random)
        {
            return new SimulationResult(
                _homeGoals,
                _awayGoals,
                _homeYellowCards,
                _awayYellowCards,
                _homeRedCards,
                _awayRedCards);
        }
    }
}