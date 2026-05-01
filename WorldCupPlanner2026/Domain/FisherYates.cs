namespace Domain;

public static class FisherYates
{
    public static List<T> Shuffle<T>(List<T> list, int seed)
    {
        var result = new List<T>(list);
        var random = new Random(seed);

        for (int i = result.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (result[i], result[j]) = (result[j], result[i]);
        }

        return result;
    }
}