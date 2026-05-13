﻿namespace Domain;

public class Team
{
    private const int MinRanking = 300;
    private const int MaxRanking = 2500;
    private const int MaxNameLength = 60;

    private static readonly HashSet<string> ValidConfederations = new()
    {
        "AFC", "CAF", "CONCACAF", "CONMEBOL", "OFC", "UEFA"
    };

    public string Name { get; private set; }
    public string ConfederationName { get; private set; }
    public int RankingFifa { get; private set; }
    public int Points { get; private set; }
    public int GoalDifference { get; private set; }
    public int GoalsInFavor { get; private set; }

    public Team(string name, string confederationName, int rankingFifa)
    {
        UpdateName(name);
        UpdateConfederationName(confederationName);
        UpdateRankingFifa(rankingFifa);
        
        Points            = 0;
        GoalDifference    = 0;
        GoalsInFavor      = 0;
    }

    public void UpdateName(string name)
    {
        var normalized = Normalized(name);

        if (InvalidNameLength(normalized))
            throw new ArgumentException($"Name must be between 1 and {MaxNameLength} characters.");

        Name = normalized;
    }

    public void UpdateRankingFifa(int rankingFifa)
    {
        if (InvalidRankingFifa(rankingFifa))
            throw new ArgumentException($"Ranking fifa must be between {MinRanking} and {MaxRanking}.");

        RankingFifa = rankingFifa;
    }

    public void UpdateConfederationName(string confederationName)
    {
        var normalized = Normalized(confederationName);

        if (string.IsNullOrWhiteSpace(normalized))
            throw new ArgumentException("Confederation is required.");

        if (!ValidConfederations.Contains(normalized))
            throw new ArgumentException("Invalid confederation.");

        ConfederationName = normalized;
    }

    public void RecordWin(int goalsScored, int goalsConceded)
    {
        UpdateStats(3, goalsScored, goalsConceded);
    }

    public void RecordDraw(int goalsScored)
    {
        UpdateStats(1, goalsScored, goalsScored);
    }

    public void RecordLoss(int goalsScored, int goalsConceded)
    {
        UpdateStats(0, goalsScored, goalsConceded);
    }

    private void UpdateStats(int points, int goalsScored, int goalsConceded)
    {
        GuardAgainstNegativeGoals(goalsScored, goalsConceded);

        Points += points;
        GoalsInFavor += goalsScored;
        GoalDifference += goalsScored - goalsConceded;
    }

    private void GuardAgainstNegativeGoals(params int[] goals)
    {
        if (goals.Any(goal => goal < 0))
            throw new ArgumentException("Goals cannot be negative.");
    }
    
    private static bool InvalidNameLength(string normalized)
    {
        return string.IsNullOrWhiteSpace(normalized) || normalized.Length > MaxNameLength;
    }
    
    private static bool InvalidRankingFifa(int rankingFifa)
    {
        return rankingFifa < MinRanking || rankingFifa > MaxRanking;
    }
    
    public static string Normalized(string name)
    {
        return name?.Trim().ToUpper() ?? string.Empty;
        
    }
    
}
