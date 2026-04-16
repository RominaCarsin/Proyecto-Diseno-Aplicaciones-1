namespace Domain;

public class Stadium
{
    public string Name { get; private set; }
    public string City { get; private set; }
    public string Description { get; private set; }
    public int Capacity { get; private set; }

    public Stadium(string name, string city, string description, int capacity)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new Exception("Stadium name cannot be empty");
        }
        
        if (name.Length > 80)
        {
            throw new Exception("Stadium name cannot exceed 80 characters.");
        }
        
        if (string.IsNullOrWhiteSpace(city))
        {
            throw new Exception("Stadium city cannot be empty.");
        }
        
        if (city.Length > 60)
        {
            throw new Exception("Stadium city cannot exceed 60 characters.");
        }
        
        if (capacity < 20000)
        {
            throw new Exception("Stadium capacity must be at least 20000.");
        }
        
        Name = name;
        City = city;
        Description = description;
        Capacity = capacity;
    }
    
    

}