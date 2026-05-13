using Domain;
using IRepository;
using IServices;

namespace Services;

public class PotService : IPotService
{
    private readonly IRepository<Team> _teamRepository;
    private readonly IRepository<Pot> _potRepository;

    public PotService(
        IRepository<Team> teamRepository,
        IRepository<Pot> potRepository)
    {
        _teamRepository = teamRepository;
        _potRepository = potRepository;
    }

    public void GeneratePots(int seed)
    {
        var teamList = _teamRepository.GetAll().ToList();

        if (teamList.Count != 48)
            throw new ArgumentException("Se requieren exactamente 48 equipos para generar los bombos.");

        SortByFifaRankingDescending(teamList);
        ShuffleTiedGroups(teamList, seed);
        CreateAndSavePots(teamList);
    }

    public IEnumerable<Pot> GetPots() => _potRepository.GetAll();

    private static void SortByFifaRankingDescending(List<Team> teams)
    {
        for (int i = 0; i < teams.Count - 1; i++)
        {
            for (int j = 0; j < teams.Count - 1 - i; j++)
            {
                if (teams[j].RankingFifa < teams[j + 1].RankingFifa)
                    (teams[j], teams[j + 1]) = (teams[j + 1], teams[j]);
            }
        }
    }

    private static void ShuffleTiedGroups(List<Team> teams, int seed)
    {
        var shuffleService = new ShuffleService(seed);
        int start = 0;

        while (start < teams.Count)
        {
            int end = start;
            while (end < teams.Count && teams[end].RankingFifa == teams[start].RankingFifa)
                end++;

            if (end - start > 1)
                shuffleService.ShuffleRange(teams, start, end);

            start = end;
        }
    }

    private void CreateAndSavePots(List<Team> teams)
    {
        for (int potNumber = 1; potNumber <= 4; potNumber++)
        {
            var potTeams = teams.Skip((potNumber - 1) * 12).Take(12).ToList();
            _potRepository.Add(new Pot(potNumber, potTeams));
        }
    }
}
