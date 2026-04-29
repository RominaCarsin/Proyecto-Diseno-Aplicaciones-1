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

    public Stadium(string name, string city, string description, int capacity)
    {
        name = name?.Trim().ToUpper() ?? string.Empty;
        city = city?.Trim().ToUpper() ?? string.Empty;
       
        
        ValidateName(name);
        ValidateCity(city);
        ValidateCapacity(capacity);
        ValidateDescription(description);
        
        Name = name;
        City = city;
        Description = description;
        Capacity = capacity;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new Exception("Stadium name cannot be empty");

        if (name.Length > MaxNameLength)
            throw new Exception($"Stadium name cannot exceed {MaxNameLength} characters");
    }
    
    private static void ValidateCity(string city)
    {
        if (string.IsNullOrWhiteSpace(city))
            throw new Exception("Stadium city cannot be empty");

        if (city.Length > MaxCityLength)
            throw new Exception($"Stadium city cannot exceed {MaxCityLength} characters");
    }
    
    private static void ValidateCapacity(int capacity)
    {
        if (capacity < MinCapacity)
            throw new Exception($"Stadium capacity must be at least {MinCapacity}");
    }

    private static void ValidateDescription(string? description)
    {
        if (description != null && description.Length > MaxDescriptionLength)
            throw new Exception($"Stadium description cannot exceed {MaxDescriptionLength} characters");
    }

}