namespace Domain;

public class Group
{
    private const char MinLabel = 'A';
    private const char MaxLabel = 'L';

    public char Label { get; private set; }

    public Group(char label)
    {
        if (label < MinLabel || label > MaxLabel)
        {
            throw new ArgumentException($"Group label must be between {MinLabel} and {MaxLabel}.");
        }

        Label = label;
    }

    public override string ToString()
    {
        return Label.ToString();
    }
}