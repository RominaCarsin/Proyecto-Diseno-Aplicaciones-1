namespace Domain;

public interface IMatchFactory
{
    Match CreateGroupMatch(
        DateTime date,
        Stadium stadium,
        Team home,
        Team away,
        char groupLabel
    );
}
