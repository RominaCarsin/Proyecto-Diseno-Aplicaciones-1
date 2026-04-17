namespace Domain;

public class User
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Password { get; set; }

    public User(string name, string surname, string email, DateTime dateOfBirth, string password)
    {
       ValidateName(name);
       ValidateSurname(surname);
       ValidateEmail(email);
       ValidateBirthDate(dateOfBirth);
       ValidatePassword(password);
       
       Name = name;
       Surname = surname;
       Email = email;
       DateOfBirth = dateOfBirth;
       Password = password;
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
    private void ValidatePassword(string password)
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