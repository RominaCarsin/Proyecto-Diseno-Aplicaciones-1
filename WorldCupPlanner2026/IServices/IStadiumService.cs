using DTOs;

namespace IServices;

public interface IStadiumService
{
    void AddStadium(StadiumRequest stadiumRequest);
    IEnumerable<StadiumResponse> GetAllStadiums();
    void DeleteStadium(string stadiumName);
    void UpdateStadium(string originalStadiumName, StadiumRequest updatedStadiumData);
}