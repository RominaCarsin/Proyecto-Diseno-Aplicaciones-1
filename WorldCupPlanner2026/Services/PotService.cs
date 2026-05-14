using Domain;
using IRepository;
using IServices;

namespace Services;

public class PotService : IPotService
{
    private const int RequiredTeamCount = 48;
    private const string InvalidTeamCountErrorMessage = "Se requieren exactamente 48 equipos para generar los bombos.";

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
        var teams = _teamRepository.GetAll().ToList();

        ValidateTeamCount(teams);
        SortByFifaRankingDescending(teams);
        ShuffleTiedRankingGroups(teams, seed);
        CreateAndSavePots(teams);
    }

    public IEnumerable<Pot> GetPots() => _potRepository.GetAll();
    

    private static void ValidateTeamCount(List<Team> teams)
    {
        if (teams.Count != RequiredTeamCount)
            throw new ArgumentException(InvalidTeamCountErrorMessage);
    }
    
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

    private static void ShuffleTiedRankingGroups(List<Team> teams, int seed)
    {
        var shuffleService = new ShuffleService(seed);
        int start = 0;

        while (start < teams.Count)
        {
            int end = FindEndOfTiedGroup(teams, start);

            bool tiedGroupHasMoreThanOneTeam = end - start > 1;
            if (tiedGroupHasMoreThanOneTeam)
                shuffleService.ShuffleRange(teams, start, end);

            start = end;
        }
    }

    private static int FindEndOfTiedGroup(List<Team> teams, int start)
    {
        int end = start;
        while (end < teams.Count && teams[end].RankingFifa == teams[start].RankingFifa)
            end++;
        return end;
    }
    
    private void CreateAndSavePots(List<Team> teams)
    {
        for (int potNumber = Pot.MinNumber; potNumber <= Pot.MaxNumber; potNumber++)
        {
            var potTeams = teams
                .Skip((potNumber - 1) * Pot.MaxTeams)
                .Take(Pot.MaxTeams)
                .ToList();

            _potRepository.Add(new Pot(potNumber, potTeams));
        }
    }
}