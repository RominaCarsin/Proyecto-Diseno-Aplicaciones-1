namespace Domain;

public class Stadium
{
    private const int MaxNameLength = 80;
    private const int MaxCityLength = 60;
    private const int MaxDescriptionLength = 400;
    private const int MinCapacity = 20000;
    
    private const string NameEmptyErrorMessage = "El nombre del estadio no puede estar vacío.";
    private const string NameTooLongErrorMessage = "El nombre del estadio no puede superar los 80 caracteres.";
    private const string CityEmptyErrorMessage = "La ciudad del estadio no puede estar vacía.";
    private const string CityTooLongErrorMessage = "La ciudad del estadio no puede superar los 60 caracteres.";
    private const string DescriptionTooLongErrorMessage = "La descripción del estadio no puede superar los 400 caracteres.";
    private const string CapacityTooSmallErrorMessage = "La capacidad del estadio debe ser de al menos 20000.";
    
    public int StadiumId { get; private set; }
    public string Name { get; private set; } = null!;
    public string City { get; private set; } = null!;
    public string? Description { get; private set; }
    public int Capacity { get; private set; }

    private Stadium() {}
    public Stadium(string name, string city, string? description, int capacity)
    {
        Name = NormalizeAndValidateName(name);
        City = NormalizeAndValidateCity(city);
        Description = NormalizeDescription(description);
        Capacity = ValidateCapacity(capacity);
    }
    
    public void UpdateName(string name)
    {
        Name = NormalizeAndValidateName(name);
    }

    public void UpdateCity(string city)
    {
        City = NormalizeAndValidateCity(city);
    }

    public void UpdateDescription(string? description)
    {
        Description = NormalizeDescription(description);
    }

    public void UpdateCapacity(int capacity)
    {
        Capacity = ValidateCapacity(capacity);
    }
    
    private static string NormalizeAndValidateName(string name) =>
        NormalizeAndValidateTextField(name, MaxNameLength, NameEmptyErrorMessage, NameTooLongErrorMessage);

    private static string NormalizeAndValidateCity(string city) =>
        NormalizeAndValidateTextField(city, MaxCityLength, CityEmptyErrorMessage, CityTooLongErrorMessage);

    private static string NormalizeAndValidateTextField(string value, int maxLength, string emptyErrorMessage, string tooLongErrorMessage)
    {
        var normalized = TextNormalizer.Normalize(value);

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

    private static int ValidateCapacity(int capacity)
    {
        if (capacity < MinCapacity)
            throw new ArgumentException(CapacityTooSmallErrorMessage);

        return capacity;
    }
}