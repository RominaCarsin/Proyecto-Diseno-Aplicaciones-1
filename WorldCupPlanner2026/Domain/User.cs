using System.ComponentModel.DataAnnotations;

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
    private const int CaesarShift = 3;
    private const int MinPasswordLength = 8;
    private const int MaxAgeInYears = 120;

    private const string NameRequiredErrorMessage = "El nombre no puede estar vacío.";
    private const string SurnameRequiredErrorMessage = "El apellido no puede estar vacío.";
    private const string EmailRequiredErrorMessage = "El email no puede estar vacío.";
    private const string EmailMissingAtSignErrorMessage = "El email debe contener @.";
    private const string EmailInvalidFormatErrorMessage = "El email debe tener texto antes y después del @.";
    private const string BirthDateRequiredErrorMessage = "La fecha de nacimiento es obligatoria.";
    private const string BirthDateTooOldErrorMessage = "La fecha de nacimiento no es realista.";
    private const string BirthDateInFutureErrorMessage = "La fecha de nacimiento no puede estar en el futuro.";
    private const string PasswordEmptyErrorMessage = "La contraseña no puede estar vacía.";
    private const string PasswordTooShortErrorMessage = "La contraseña debe tener al menos 8 caracteres.";
    private const string PasswordMissingLowercaseErrorMessage = "La contraseña debe contener una letra minúscula.";
    private const string PasswordMissingUppercaseErrorMessage = "La contraseña debe contener una letra mayúscula.";
    private const string PasswordMissingDigitErrorMessage = "La contraseña debe contener un número.";
    private const string PasswordMissingSpecialCharErrorMessage = "La contraseña debe contener un carácter especial.";
    
    public int UserId { get; private set; }
    public string Name { get; private set; }
    public string Surname { get; private set; }
    
    public string Email { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public string PasswordHash { get; private set; } = string.Empty;
    public Role Roles { get; private set; }

    private User(){}
    public User(
        string name,
        string surname,
        string email,
        DateTime dateOfBirth,
        string plainPassword,
        Role roles = Role.None)
    {
        UpdatePersonalInfo(name, surname, email, dateOfBirth);
        Roles = roles;
        SetPassword(plainPassword);
    }

    public void UpdatePersonalInfo(string name, string surname, string email, DateTime dateOfBirth)
    {
        ValidateName(name);
        ValidateSurname(surname);
        ValidateEmail(email);
        ValidateBirthDate(dateOfBirth);

        Name = name;
        Surname = surname;
        Email = email;
        DateOfBirth = dateOfBirth;
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

        return DecryptPassword(PasswordHash, CaesarShift) == plainPassword;
    }

    public void AddRole(Role role) => Roles |= role;
    public void RemoveRole(Role role) => Roles &= ~role;
    public bool HasRole(Role role) => (Roles & role) == role;
    
    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(NameRequiredErrorMessage);
    }

    private static void ValidateSurname(string surname)
    {
        if (string.IsNullOrWhiteSpace(surname))
            throw new ArgumentException(SurnameRequiredErrorMessage);
    }

    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException(EmailRequiredErrorMessage);

        if (!email.Contains('@'))
            throw new ArgumentException(EmailMissingAtSignErrorMessage);

        var parts = email.Split('@');
        bool hasTextBeforeAndAfterAtSign = parts.Length == 2
            && !string.IsNullOrWhiteSpace(parts[0])
            && !string.IsNullOrWhiteSpace(parts[1]);

        if (!hasTextBeforeAndAfterAtSign)
            throw new ArgumentException(EmailInvalidFormatErrorMessage);
    }

    private static void ValidateBirthDate(DateTime birthDate)
    {
        var today = DateTime.Now;

        if (birthDate == DateTime.MinValue)
            throw new ArgumentException(BirthDateRequiredErrorMessage);

        if (birthDate < today.AddYears(-MaxAgeInYears))
            throw new ArgumentException(BirthDateTooOldErrorMessage);

        if (birthDate > today)
            throw new ArgumentException(BirthDateInFutureErrorMessage);
    }

    private static void ValidatePasswordComplexity(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException(PasswordEmptyErrorMessage);

        if (password.Length < MinPasswordLength)
            throw new ArgumentException(PasswordTooShortErrorMessage);

        if (!password.Any(char.IsLower))
            throw new ArgumentException(PasswordMissingLowercaseErrorMessage);

        if (!password.Any(char.IsUpper))
            throw new ArgumentException(PasswordMissingUppercaseErrorMessage);

        if (!password.Any(char.IsDigit))
            throw new ArgumentException(PasswordMissingDigitErrorMessage);

        if (!password.Any(c => !char.IsLetterOrDigit(c)))
            throw new ArgumentException(PasswordMissingSpecialCharErrorMessage);
    }

    private static string EncryptPassword(string password, int shift) =>
        new string(password.Select(c => (char)(c + shift)).ToArray());

    private static string DecryptPassword(string cipherText, int shift) =>
        new string(cipherText.Select(c => (char)(c - shift)).ToArray());
}