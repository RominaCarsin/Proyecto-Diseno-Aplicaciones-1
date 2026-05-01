namespace IServices;

public interface IShuffleService
{
    void Shuffle<T>(IList<T> list);
}