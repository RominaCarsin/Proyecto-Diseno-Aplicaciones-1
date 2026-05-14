using DTOs;

namespace IServices;

public interface IFixtureService
{
    void GenerateFixture(int seed, DateTime startDate);
    bool CanGenerateFixture();
    int GetStadiumCount();
    void SimulateGroupStage(int seed);
    void InitializeKnockoutStage(int simulationSeed, int semillaCrucesFase = 0);
    void InitializeRoundOf16();
    void InitializeQuarterFinals();
    void InitializeSemiFinals();
    void InitializeThirdPlace();
    void InitializeFinal();
}