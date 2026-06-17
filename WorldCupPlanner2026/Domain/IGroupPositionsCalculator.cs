namespace Domain;

public interface IGroupPositionsCalculator
{
    IEnumerable<Team> Order(IReadOnlyList<Team> teams);
}