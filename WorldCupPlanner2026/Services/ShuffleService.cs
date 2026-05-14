using Domain;

namespace Services;

public class ShuffleService : Domain.IShuffleService
{
    private readonly Random _random;

    public ShuffleService(int seed) => _random = new Random(seed);

    public void Shuffle<T>(IList<T> list) =>
        ShuffleRange(list, 0, list.Count);

    public void ShuffleByRanking(IList<Team> teams)
    {
        int start = 0;

        while (start < teams.Count)
        {
            int end = FindEndOfTiedGroup(teams, start);

            if (end - start > 1)
                ShuffleRange(teams, start, end);

            start = end;
        }
    }

    private void ShuffleRange<T>(IList<T> list, int start, int end)
    {
        for (int i = end - 1; i > start; i--)
        {
            int j = _random.Next(start, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    private static int FindEndOfTiedGroup(IList<Team> teams, int start)
    {
        int end = start;
        while (end < teams.Count && teams[end].RankingFifa == teams[start].RankingFifa)
            end++;
        return end;
    }
}