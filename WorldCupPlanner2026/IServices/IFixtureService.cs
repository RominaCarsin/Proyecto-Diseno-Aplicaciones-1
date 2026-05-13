using DTOs;

namespace IServices;

public interface IFixtureService
{
    void GenerateFixture(int seed, DateTime startDate);
    bool CanGenerateFixture();
    int GetStadiumCount();
    void SimulateGroupStage(int seed);
}