namespace Domain;

public class Shuffler : IShuffler
{
    private readonly Random _random;

    public Shuffler(int seed) => _random = new Random(seed);

    public void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
