using Domain;

namespace IServices;

public interface IPotService
{
    void GeneratePots(int seed);
    IEnumerable<Pot> GetPots();
}