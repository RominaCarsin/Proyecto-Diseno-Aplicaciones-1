namespace Domain;

public class Stadium
{
    private const int MaxNameLength = 80;
    private const int MaxCityLength = 60;
    private const int MaxDescriptionLength = 400;
    private const int MinCapacity = 20000;

    private const string NameEmptyErrorMessage = "Stadium name cannot be empty.";
    private const string NameTooLongErrorMessage = "Stadium name cannot exceed 80 characters.";
    private const string CityEmptyErrorMessage = "Stadium city cannot be empty.";
    private const string CityTooLongErrorMessage = "Stadium city cannot exceed 60 characters.";
    private const string DescriptionTooLongErrorMessage = "Stadium description cannot exceed 400 characters.";
    private const string CapacityTooSmallErrorMessage = "Stadium capacity must be at least 20000.";

    public string Name { get; private set; }
    public string City { get; private set; }
    public string? Description { get; private set; }
    public int Capacity { get; private set; }

    public Stadium(string name, string city, string? description, int capacity)
    {
        Name = NormalizeAndValidateName(name);
        City = NormalizeAndValidateCity(city);
        Description = NormalizeDescription(description);
        Capacity = ValidatedCapacity(capacity);
    }

    // ─── Validaciones ─────────────────────────────────────────────────────────

    private static string NormalizeAndValidateName(string name) =>
        NormalizeAndValidateTextField(name, MaxNameLength, NameEmptyErrorMessage, NameTooLongErrorMessage);

    private static string NormalizeAndValidateCity(string city) =>
        NormalizeAndValidateTextField(city, MaxCityLength, CityEmptyErrorMessage, CityTooLongErrorMessage);

    private static string NormalizeAndValidateTextField(
        string value,
        int maxLength,
        string emptyErrorMessage,
        string tooLongErrorMessage)
    {
        var normalized = Normalize(value);

        if (string.IsNullOrWhiteSpace(normalized))
            throw new ArgumentException(emptyErrorMessage);

        if (normalized.Length > maxLength)
            throw new ArgumentException(tooLongErrorMessage);

        return normalized;
    }

    private static string? NormalizeDescription(string? description)
    {
        if (description is null)
            return null;

        if (description.Length > MaxDescriptionLength)
            throw new ArgumentException(DescriptionTooLongErrorMessage);

        return description;
    }

    private static int ValidatedCapacity(int capacity)
    {
        if (capacity < MinCapacity)
            throw new ArgumentException(CapacityTooSmallErrorMessage);

        return capacity;
    }

    // ─── Helpers ──────────────────────────────────────────────────────────────

    private static string Normalize(string value) =>
        value?.Trim().ToUpper() ?? string.Empty;
}