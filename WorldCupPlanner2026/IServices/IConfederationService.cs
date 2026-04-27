using Domain;
using IRepository;

namespace IServices;

public interface IConfederationService
{
    IEnumerable<Confederation> GetAll();
    void AddTeamToConfederation(Team team);
}