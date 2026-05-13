namespace DTOs;

public class TeamImportResult
{
    public int TotalRows { get; set; }
    public int SuccessfulRows { get; set; }
    public int FailedRows { get; set; }
    public List<TeamImportRowResult> Rows { get; set; } = new();
}

public class TeamImportRowResult
{
    public int RowNumber { get; set; }
    public bool Success { get; set; }
    public TeamResponse? Team { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}
