namespace Domain;

public static class TextNormalizer
{
    public static string Normalize(string? value)
    {
        return value?.Trim().ToUpper() ?? string.Empty;
    }
}