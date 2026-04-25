using Domain;
using IServices;

namespace Services;

public class ConfederationService : IConfederationService
{
    private readonly List<Confederation> _confederations = new()
    {
        new Confederation("UEFA",     16),
        new Confederation("CAF",       9),
        new Confederation("AFC",       8),
        new Confederation("CONCACAF",  7),
        new Confederation("CONMEBOL",  7),
        new Confederation("OFC",       1),
    };

    public IEnumerable<Confederation> GetAll() => _confederations;
}