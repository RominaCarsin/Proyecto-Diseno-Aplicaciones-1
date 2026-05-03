namespace IServices;

public interface IShuffleService
{
    void Shuffle<T>(IList<T> list);
    void ShuffleRange<T>(IList<T> list, int start, int end);
}