namespace Domain;

public interface IShuffler
{
    void Shuffle<T>(IList<T> list);
}
