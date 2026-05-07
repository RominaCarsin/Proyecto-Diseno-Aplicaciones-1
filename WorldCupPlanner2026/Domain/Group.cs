namespace Domain;

using System;
using System.Collections.Generic;

public class Group
{
    private const string LabelOutOfRangeErrorMessage = "Label debe ser una letra mayúscula entre 'A' y 'L'.";

    public char Label { get; private set; }
    public List<Team> Teams { get; }

    public Group(char label)
    {
        UpdateLabel(label);
        Teams = new List<Team>();
    }

    public void UpdateLabel(char label)
    {
        if (label < 'A' || label > 'L')
            throw new ArgumentException(LabelOutOfRangeErrorMessage, nameof(label));

        Label = char.ToUpperInvariant(label);
    }
}