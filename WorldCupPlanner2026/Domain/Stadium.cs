namespace Domain;

public class Stadium
{
    private const int MaxNameLength = 80;
    private const int MaxCityLength = 60;
    private const int MaxDescriptionLength = 400;
    private const int MinCapacity = 20000;

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

    private static string NormalizeAndValidateName(string name)
    {
        var normalized = Normalize(name);

        if (string.IsNullOrWhiteSpace(normalized))
            throw new ArgumentException("Stadium name cannot be empty");

        if (normalized.Length > MaxNameLength)
            throw new ArgumentException($"Stadium name cannot exceed {MaxNameLength} characters");

        return normalized;
    }

    private static string NormalizeAndValidateCity(string city)
    {
        var normalized = Normalize(city);

        if (string.IsNullOrWhiteSpace(normalized))
            throw new ArgumentException("Stadium city cannot be empty");

        if (normalized.Length > MaxCityLength)
            throw new ArgumentException($"Stadium city cannot exceed {MaxCityLength} characters");

        return normalized;
    }

    private static string? NormalizeDescription(string? description)
    {
        if (description is null)
            return null;

        if (description.Length > MaxDescriptionLength)
            throw new ArgumentException($"Stadium description cannot exceed {MaxDescriptionLength} characters");

        return description;
    }

    private static int ValidatedCapacity(int capacity)
    {
        if (capacity < MinCapacity)
            throw new ArgumentException($"Stadium capacity must be at least {MinCapacity}");

        return capacity;
    }

    private static string Normalize(string value) =>
        value?.Trim().ToUpper() ?? string.Empty;
}
