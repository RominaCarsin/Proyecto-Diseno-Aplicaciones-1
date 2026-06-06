using Domain;

namespace IServices;

public interface IUpdateRankingService
{
    void UpdateAfterMatch(Match match);
}