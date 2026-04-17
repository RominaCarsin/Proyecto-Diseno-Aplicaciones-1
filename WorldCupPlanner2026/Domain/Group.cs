namespace Domain;

public class Group
{
    public char Label { get; }

    public Group(char label)
    {
        if (label < 'A' || label > 'L')
        {
            throw new Exception("Group label must be between A and L.");
        }
        Label = label;
    }
}