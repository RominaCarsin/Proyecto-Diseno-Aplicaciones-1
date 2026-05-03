namespace Services;

public class ShuffleService
{
    private readonly Random _random;

    public ShuffleService(int seed)
    {
        _random = new Random(seed);
    }

    public void Shuffle<T>(IList<T> list)
    {
        ShuffleRange(list, 0, list.Count);
    }

    public void ShuffleRange<T>(IList<T> list, int start, int end)
    {
        for (int i = end - 1; i > start; i--)
        {
            int j = _random.Next(start, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}