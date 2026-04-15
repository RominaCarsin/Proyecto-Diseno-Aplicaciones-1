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
        Name = name;
        City = city;
        Description = description;
        Capacity = capacity;
    }
    
    

}