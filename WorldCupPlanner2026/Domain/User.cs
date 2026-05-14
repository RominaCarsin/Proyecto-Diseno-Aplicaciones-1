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

    private const string NameRequiredErrorMessage = "Name cannot be empty.";
    private const string SurnameRequiredErrorMessage = "Surname cannot be empty.";
    private const string EmailRequiredErrorMessage = "Email cannot be empty.";
    private const string EmailMissingAtSignErrorMessage = "Email must contain @.";
    private const string EmailInvalidFormatErrorMessage = "Email must have text before and after @.";
    private const string BirthDateRequiredErrorMessage = "Date of birth is required.";
    private const string BirthDateTooOldErrorMessage = "Date of birth is not realistic.";
    private const string BirthDateInFutureErrorMessage = "Date of birth cannot be in the future.";
    private const string PasswordEmptyErrorMessage = "Password cannot be empty.";
    private const string PasswordTooShortErrorMessage = "Password must be at least 8 characters long.";
    private const string PasswordMissingLowercaseErrorMessage = "Password must contain a lowercase letter.";
    private const string PasswordMissingUppercaseErrorMessage = "Password must contain an uppercase letter.";
    private const string PasswordMissingDigitErrorMessage = "Password must contain a number.";
    private const string PasswordMissingSpecialCharErrorMessage = "Password must contain a special character.";

    public string Name { get; private set; }
    public string Surname { get; private set; }
    public string Email { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public string PasswordHash { get; private set; } = string.Empty;
    public Role Roles { get; private set; }

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