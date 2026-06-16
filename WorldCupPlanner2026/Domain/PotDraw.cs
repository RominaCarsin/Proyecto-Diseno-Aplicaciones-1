namespace Domain;

public class PotDraw
{
    public void ShuffleByRanking(IList<Team> teams, int seed)
    {
        var random = new Random(seed);
        int start = 0;

        while (start < teams.Count)
        {
            int end = FindEndOfTiedGroup(teams, start);

            if (end - start > 1)
                FisherYatesRange(teams, start, end, random);

            start = end;
        }
    }

    private static void FisherYatesRange<T>(IList<T> list, int start, int end, Random random)
    {
        for (int i = end - 1; i > start; i--)
        {
            int j = random.Next(start, i + 1);
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
