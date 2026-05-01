using System;
using System.Linq;

namespace Domain;

[Flags]
public enum Role
{
    None = 0,
    Admin = 1,
    Editor = 2
}

public class User
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string PasswordHash { get; private set; } = string.Empty;
    public Role Roles { get; private set; }

    private const int CaesarShift = 3;

    public User(string name, string surname, string email, DateTime dateOfBirth, string plainPassword, Role roles = Role.None)
    {
       ValidateName(name);
       ValidateSurname(surname);
       ValidateEmail(email);
       ValidateBirthDate(dateOfBirth);
       ValidatePasswordComplexity(plainPassword);

       Name = name;
       Surname = surname;
       Email = email;
       DateOfBirth = dateOfBirth;
       Roles = roles;

       SetPassword(plainPassword);
    }

    public void SetPassword(string plainPassword)
    {
        ValidatePasswordComplexity(plainPassword);

        PasswordHash = EncryptPassword(plainPassword, CaesarShift);
    }

    public bool VerifyPassword(string plainPassword)
    {
        if (string.IsNullOrEmpty(PasswordHash))
            return false;

        var decrypted = DecryptPassword(PasswordHash, CaesarShift);
        return decrypted == plainPassword;
    }

    public void AddRole(Role role) => Roles |= role;
    public void RemoveRole(Role role) => Roles &= ~role;
    public bool HasRole(Role role) => (Roles & role) == role;

    private static string EncryptPassword(string password, int shift)
    {
        return new string(password.Select(c => (char)(c + shift)).ToArray());
    }

    private static string DecryptPassword(string cipherText, int shift)
    {
        return new string(cipherText.Select(c => (char)(c - shift)).ToArray());
    }

    private void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty");
    }

    private void ValidateSurname(string surname)
    {
        if (string.IsNullOrWhiteSpace(surname))
            throw new ArgumentException("Surname cannot be empty");
    }
    
    private void  ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty");
        if (!email.Contains("@"))
            throw new ArgumentException("Email must contain @");

        var parts = email.Split('@');
        if (parts.Length != 2 ||
            string.IsNullOrWhiteSpace(parts[0]) ||
            string.IsNullOrWhiteSpace(parts[1]))
        {
            throw new ArgumentException("Email must have text before and after @");
        }
    }

    private void ValidateBirthDate(DateTime birthDate)
    {
        if (birthDate < DateTime.Now.AddYears(-120))
            throw new ArgumentException("Date of birth is not realistic");
        if (birthDate == DateTime.MinValue)
            throw new ArgumentException("Date of birth is required");
        if (birthDate > DateTime.Now)
            throw new ArgumentException("Date of birth cannot be in the future");
    }

    private void ValidatePasswordComplexity(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password cannot be empty");
        if (password.Length < 8)
            throw new ArgumentException("Password must be at least 8 characters long");
        if (!password.Any(char.IsLower))
            throw new ArgumentException("Password must contain a lowercase letter");
        if (!password.Any(char.IsUpper))
            throw new ArgumentException("Password must contain an uppercase letter");
        if (!password.Any(char.IsDigit))
            throw new ArgumentException("Password must contain a number");
        if (!password.Any(c => !char.IsLetterOrDigit(c)))
            throw new ArgumentException("Password must contain a special character");
    }

}