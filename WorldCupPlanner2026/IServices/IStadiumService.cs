using Domain;
using DTOs;

namespace IServices;

public interface IStadiumService
{
    void AddStadium(StadiumRequest stadium);
    IEnumerable<StadiumRequest> GetStadiums();
    void DeleteStadium(string stadiumName);
    void UpdateStadium(string originalName, StadiumRequest stadium);
}

