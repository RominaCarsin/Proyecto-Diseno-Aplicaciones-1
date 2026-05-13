using DTOs;

namespace IServices;

public interface ITeamImportService
{
    TeamImportResult Preview(string csvContent);
    TeamImportResult Import(string csvContent);
}
