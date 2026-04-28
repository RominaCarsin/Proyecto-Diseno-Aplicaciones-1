using Domain;

namespace IServices;

public interface IStadiumService
{
    void AddStadium(Stadium stadium);
    IEnumerable<Stadium> GetStadiums();
    void DeleteStadium(string stadiumName);
    void UpdateStadium(string originalName, Stadium stadium);
}

