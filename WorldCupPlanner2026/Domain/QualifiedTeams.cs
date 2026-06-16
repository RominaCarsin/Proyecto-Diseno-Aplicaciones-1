namespace Domain;

public class QualifiedTeams
{
    private const int ExpectedFirstPlacesCount = 12;
    private const int ExpectedSecondPlacesCount = 12;
    private const int ExpectedThirdPlacesCount = 8;
    private const int BestFirstPlacesCount = 4;
    private const int BestSecondPlacesCount = 8;

    private const string NullFirstPlacesErrorMessage = "La lista de primeros puestos no puede ser nula.";
    private const string NullSecondPlacesErrorMessage = "La lista de segundos puestos no puede ser nula.";
    private const string NullThirdPlacesErrorMessage = "La lista de terceros puestos no puede ser nula.";
    private const string FirstPlacesCountErrorMessage = "Debe haber exactamente 12 primeros puestos.";
    private const string SecondPlacesCountErrorMessage = "Debe haber exactamente 12 segundos puestos.";
    private const string ThirdPlacesCountErrorMessage = "Debe haber exactamente 8 mejores terceros.";

    public IReadOnlyList<Team> FirstPlaces { get; }
    public IReadOnlyList<Team> SecondPlaces { get; }
    public IReadOnlyList<Team> BestThirdPlaces { get; }

    public IReadOnlyList<Team> BestFirstPlaces => FirstPlaces.Take(BestFirstPlacesCount).ToList();
    public IReadOnlyList<Team> WorstFirstPlaces => FirstPlaces.Skip(BestFirstPlacesCount).ToList();
    public IReadOnlyList<Team> BestSecondPlaces => SecondPlaces.Take(BestSecondPlacesCount).ToList();
    public IReadOnlyList<Team> WorstSecondPlaces => SecondPlaces.Skip(BestSecondPlacesCount).ToList();
    public IReadOnlyList<Team> ThirdPlaces => BestThirdPlaces;

    public QualifiedTeams(
        IReadOnlyList<Team> firstPlaces,
        IReadOnlyList<Team> secondPlaces,
        IReadOnlyList<Team> bestThirdPlaces)
    {
        if (firstPlaces is null)
            throw new ArgumentNullException(nameof(firstPlaces), NullFirstPlacesErrorMessage);
        if (secondPlaces is null)
            throw new ArgumentNullException(nameof(secondPlaces), NullSecondPlacesErrorMessage);
        if (bestThirdPlaces is null)
            throw new ArgumentNullException(nameof(bestThirdPlaces), NullThirdPlacesErrorMessage);

        if (firstPlaces.Count != ExpectedFirstPlacesCount)
            throw new ArgumentException(FirstPlacesCountErrorMessage, nameof(firstPlaces));
        if (secondPlaces.Count != ExpectedSecondPlacesCount)
            throw new ArgumentException(SecondPlacesCountErrorMessage, nameof(secondPlaces));
        if (bestThirdPlaces.Count != ExpectedThirdPlacesCount)
            throw new ArgumentException(ThirdPlacesCountErrorMessage, nameof(bestThirdPlaces));

        FirstPlaces = firstPlaces;
        SecondPlaces = secondPlaces;
        BestThirdPlaces = bestThirdPlaces;
    }
}
