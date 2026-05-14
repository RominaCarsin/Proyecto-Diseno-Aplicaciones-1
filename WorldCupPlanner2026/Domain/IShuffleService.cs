namespace Domain;

public interface IShuffleService
{
    void Shuffle<T>(IList<T> list);
    void ShuffleByRanking(IList<Team> teams);
}
