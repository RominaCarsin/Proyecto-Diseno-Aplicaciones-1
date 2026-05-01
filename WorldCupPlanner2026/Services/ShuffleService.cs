
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
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = _random.Next(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}