using Domain;
using DTOs;

namespace IServices;

public interface IConfederationService
{
    IEnumerable<ConfederationResponse> GetAll();
    void AddTeamToConfederation(Team team);
    void RemoveTeamFromConfederation(Team team);
    void UpdateTeamConfederation(Team team, string newConfederationName);
}