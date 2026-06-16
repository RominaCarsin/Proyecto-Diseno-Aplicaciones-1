namespace Domain;

public class PasswordHasher : IPasswordHasher
{
    private const int Shift = 3;

    public string Hash(string plainPassword) =>
        new string(plainPassword.Select(c => (char)(c + Shift)).ToArray());

    public bool Verify(string plainPassword, string passwordHash)
    {
        if (string.IsNullOrEmpty(passwordHash))
            return false;

        var decrypted = new string(passwordHash.Select(c => (char)(c - Shift)).ToArray());
        return decrypted == plainPassword;
    }
}
