using Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Test;

[TestClass]
public class QualifiedTeamsTests
{
    private static List<Team> CreateTeams(int count, string groupLabel = "A") =>
        Enumerable.Range(1, count)
            .Select(i => new Team($"TEAM-{i:00}", "UEFA", 1000 + i, groupLabel))
            .ToList();

    [TestMethod]
    public void GivenValidLists_WhenConstructingQualifiedTeams_ThenPropertiesAreSet()
    {
        var firstPlaces = CreateTeams(12, "A");
        var secondPlaces = CreateTeams(12, "B");
        var thirdPlaces = CreateTeams(8, "C");

        var qt = new QualifiedTeams(firstPlaces, secondPlaces, thirdPlaces);

        Assert.AreSame(firstPlaces, qt.FirstPlaces);
        Assert.AreSame(secondPlaces, qt.SecondPlaces);
        Assert.AreSame(thirdPlaces, qt.BestThirdPlaces);
    }

    [TestMethod]
    public void GivenNullFirstPlaces_WhenConstructingQualifiedTeams_ThenThrowsArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            new QualifiedTeams(null!, CreateTeams(12), CreateTeams(8)));
    }

    [TestMethod]
    public void GivenNullSecondPlaces_WhenConstructingQualifiedTeams_ThenThrowsArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            new QualifiedTeams(CreateTeams(12), null!, CreateTeams(8)));
    }

    [TestMethod]
    public void GivenNullThirdPlaces_WhenConstructingQualifiedTeams_ThenThrowsArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            new QualifiedTeams(CreateTeams(12), CreateTeams(12), null!));
    }

    [TestMethod]
    public void GivenQualifiedTeams_WhenAccessingBestFirstPlaces_ThenReturnsFirst4()
    {
        var firstPlaces = CreateTeams(12, "A");
        var qt = new QualifiedTeams(firstPlaces, CreateTeams(12, "B"), CreateTeams(8, "C"));

        var best = qt.BestFirstPlaces;

        Assert.AreEqual(4, best.Count);
        Assert.AreSame(firstPlaces[0], best[0]);
        Assert.AreSame(firstPlaces[3], best[3]);
    }

    [TestMethod]
    public void GivenQualifiedTeams_WhenAccessingWorstFirstPlaces_ThenReturnsLast8()
    {
        var firstPlaces = CreateTeams(12, "A");
        var qt = new QualifiedTeams(firstPlaces, CreateTeams(12, "B"), CreateTeams(8, "C"));

        var worst = qt.WorstFirstPlaces;

        Assert.AreEqual(8, worst.Count);
        Assert.AreSame(firstPlaces[4], worst[0]);
        Assert.AreSame(firstPlaces[11], worst[7]);
    }

    [TestMethod]
    public void GivenQualifiedTeams_WhenAccessingBestSecondPlaces_ThenReturnsFirst8()
    {
        var secondPlaces = CreateTeams(12, "B");
        var qt = new QualifiedTeams(CreateTeams(12, "A"), secondPlaces, CreateTeams(8, "C"));

        var best = qt.BestSecondPlaces;

        Assert.AreEqual(8, best.Count);
        Assert.AreSame(secondPlaces[0], best[0]);
        Assert.AreSame(secondPlaces[7], best[7]);
    }

    [TestMethod]
    public void GivenQualifiedTeams_WhenAccessingWorstSecondPlaces_ThenReturnsLast4()
    {
        var secondPlaces = CreateTeams(12, "B");
        var qt = new QualifiedTeams(CreateTeams(12, "A"), secondPlaces, CreateTeams(8, "C"));

        var worst = qt.WorstSecondPlaces;

        Assert.AreEqual(4, worst.Count);
        Assert.AreSame(secondPlaces[8], worst[0]);
        Assert.AreSame(secondPlaces[11], worst[3]);
    }

    [TestMethod]
    public void GivenQualifiedTeams_WhenAccessingThirdPlaces_ThenReturnsSameAsBestThirdPlaces()
    {
        var thirdPlaces = CreateTeams(8, "C");
        var qt = new QualifiedTeams(CreateTeams(12, "A"), CreateTeams(12, "B"), thirdPlaces);

        Assert.AreSame(qt.BestThirdPlaces, qt.ThirdPlaces);
    }

    [TestMethod]
    public void GivenQualifiedTeams_WhenAccessingDerivedProperties_ThenContainAllTeamsWithoutOverlap()
    {
        var firstPlaces = CreateTeams(12, "A");
        var secondPlaces = CreateTeams(12, "B");
        var thirdPlaces = CreateTeams(8, "C");
        var qt = new QualifiedTeams(firstPlaces, secondPlaces, thirdPlaces);

        var bestFirst = qt.BestFirstPlaces.ToList();
        var worstFirst = qt.WorstFirstPlaces.ToList();
        var bestSecond = qt.BestSecondPlaces.ToList();
        var worstSecond = qt.WorstSecondPlaces.ToList();

        Assert.AreEqual(4, bestFirst.Count);
        Assert.AreEqual(8, worstFirst.Count);
        Assert.AreEqual(8, bestSecond.Count);
        Assert.AreEqual(4, worstSecond.Count);

        Assert.IsFalse(bestFirst.Intersect(worstFirst).Any());
        Assert.IsFalse(bestSecond.Intersect(worstSecond).Any());
    }
}
