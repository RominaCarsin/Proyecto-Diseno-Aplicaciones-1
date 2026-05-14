using Domain;
using IServices;

namespace Services;

public class ShuffleService : IServices.IShuffleService, Domain.IShuffleService
{
    private readonly Random _random;

    public ShuffleService(int seed)
    {
        _random = new Random(seed);
    }

    public void Shuffle<T>(IList<T> list)
    {
        ShuffleRange(list, startIndex: 0, endIndex: list.Count);
    }

    public void ShuffleRange<T>(IList<T> list, int startIndex, int endIndex)
    {
        for (int i = endIndex - 1; i > startIndex; i--)
        {
            int randomIndex = _random.Next(startIndex, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }
}