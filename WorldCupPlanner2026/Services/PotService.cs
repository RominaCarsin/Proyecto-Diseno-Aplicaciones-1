namespace Services;

using Domain;
using IRepository;

public class PotService
{
    private readonly IRepository<Team> _teamRepository;
    private readonly IRepository<Pot> _potRepository;
    private readonly ShuffleService _shuffleService;

    public PotService(
        IRepository<Team> teamRepository,
        IRepository<Pot> potRepository,
        ShuffleService shuffleService)
    {
        _teamRepository = teamRepository;
        _potRepository = potRepository;
        _shuffleService = shuffleService;
    }

    public void GeneratePots()
    {
        var teamList = new List<Team>();
        foreach (var team in _teamRepository.GetAll())
            teamList.Add(team);

        if (teamList.Count != 48)
            throw new ArgumentException("Se requieren exactamente 48 equipos para generar los bombos.");

        SortByFifaRankingDescending(teamList);
        ShuffleTiedGroups(teamList);
        CreateAndSavePots(teamList);
    }

    // ─── Privados ─────────────────────────────────────────────────────────────

    private void SortByFifaRankingDescending(List<Team> teams)
    {
        for (int i = 0; i < teams.Count - 1; i++)
        {
            for (int j = 0; j < teams.Count - 1 - i; j++)
            {
                if (teams[j].RankingFifa < teams[j + 1].RankingFifa)
                {
                    var temp = teams[j];
                    teams[j] = teams[j + 1];
                    teams[j + 1] = temp;
                }
            }
        }
    }

    private void ShuffleTiedGroups(List<Team> teams)
    {
        int start = 0;
        while (start < teams.Count)
        {
            int end = start;
            while (end < teams.Count && teams[end].RankingFifa == teams[start].RankingFifa)
                end++;

            if (end - start > 1)
                _shuffleService.ShuffleRange(teams, start, end);

            start = end;
        }
    }

    private void CreateAndSavePots(List<Team> teams)
    {
        for (int potNumber = 1; potNumber <= 4; potNumber++)
        {
            var potTeams = new List<Team>();
            int from = (potNumber - 1) * 12;

            for (int i = from; i < from + 12; i++)
                potTeams.Add(teams[i]);

            _potRepository.Add(new Pot(potNumber, potTeams));
        }
    }
}