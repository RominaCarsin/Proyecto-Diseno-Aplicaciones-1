using Domain;
using DTOs;
using IServices;
using IRepository;
using Services.Mappers;

namespace Services;

public class UserService : IUserService
{
    private const string UserAlreadyExistsErrorMessage = "El usuario ya existe.";
    private const string UserNotFoundErrorMessage = "No se encontró el usuario.";
    private const string EmailAlreadyInUseErrorMessage = "El email ya está en uso.";
    private const string InvalidPasswordErrorMessage = "La contraseña es inválida.";
    private const string CurrentPasswordIncorrectErrorMessage = "La contraseña actual es incorrecta.";
    private const string UserNotAuthorizedErrorMessage = "El usuario no está autorizado.";
    
    private readonly IRepository<User> _repository;
    private readonly IAuditService _auditService;

    public UserService(IRepository<User> repository, IAuditService auditService)
    {
        _repository = repository;
        _auditService = auditService;
        
        /*if (!_repository.GetAll().Any())
        {
            var adminDefault = new User(
                name: "Admin",
                surname: "Principal",
                email: "admin@test.com",
                dateOfBirth: new DateTime(2000, 1, 1),
                plainPassword: "Password1!",
                roles: Role.Admin | Role.Editor
            )
            _repository.Add(adminDefault);
        }*/
    }

    public UserResponse AddUser(UserRequest userRequest)
    {
        if (FindByEmail(userRequest.Email) is not null)
            throw new ArgumentException(UserAlreadyExistsErrorMessage);

        var user = UserMapper.ToEntity(userRequest);
        _repository.Add(user);

        _auditService.Log(userRequest.Email, "User created",
            $"New user registered: {userRequest.Name} {userRequest.Surname} ({userRequest.Email})");

        return UserMapper.FromEntity(user);
    }

    public void UpdateUser(string email, UserRequest userRequest)
    {
        var existingUser = GetUserByEmailOrThrow(email);

        ValidateEmailIsNotTakenByAnotherUser(email, userRequest.Email);

        existingUser.UpdatePersonalInfo(
            userRequest.Name,
            userRequest.Surname,
            userRequest.Email,
            userRequest.DateOfBirth);

        _auditService.Log(email, "User updated",
            $"User {email} updated: Name={userRequest.Name}, Surname={userRequest.Surname}, " +
            $"Email={userRequest.Email}, DOB={userRequest.DateOfBirth:yyyy-MM-dd}");
    }

    public void DeleteUser(string email)
    {
        var existingUser = GetUserByEmailOrThrow(email);
        _repository.Delete(existingUser);

        _auditService.Log("system", "User deleted",
            $"User {email} was removed from the system");
    }

    public UserResponse GetUserByEmail(string email)
    {
        var user = GetUserByEmailOrThrow(email);
        return UserMapper.FromEntity(user);
    }

    public IEnumerable<UserResponse> GetUsers()
    {
        return _repository.GetAll().Select(UserMapper.FromEntity);
    }

    public UserResponse Login(string email, string password)
    {
        var user = GetUserByEmailOrThrow(email);

        if (!user.VerifyPassword(password))
            throw new ArgumentException(InvalidPasswordErrorMessage);

        return UserMapper.FromEntity(user);
    }

    public bool UserHasRole(string email, RoleDto role)
    {
        return GetUserByEmailOrThrow(email)
            .HasRole(RoleMapper.ToDomain(role));
    }

    public void AddRoleToUser(string adminEmail, string targetEmail, RoleDto role)
    {
        EnsureUserIsAdmin(adminEmail);

        var targetUser = GetUserByEmailOrThrow(targetEmail);
        targetUser.AddRole(RoleMapper.ToDomain(role));

        _auditService.Log(adminEmail, "Role added",
            $"Role {role} granted to {targetEmail} by {adminEmail}");
    }

    public void RemoveRoleFromUser(string adminEmail, string targetEmail, RoleDto role)
    {
        EnsureUserIsAdmin(adminEmail);

        var targetUser = GetUserByEmailOrThrow(targetEmail);
        targetUser.RemoveRole(RoleMapper.ToDomain(role));

        _auditService.Log(adminEmail, "Role revoked",
            $"Role {role} revoked from {targetEmail} by {adminEmail}");
    }

    public void ChangePassword(string email, string currentPassword, string newPassword)
    {
        var user = GetUserByEmailOrThrow(email);

        if (!user.VerifyPassword(currentPassword))
            throw new ArgumentException(CurrentPasswordIncorrectErrorMessage);

        user.SetPassword(newPassword);

        _auditService.Log(email, "Password changed",
            $"User {email} changed their own password");
    }

    public void ResetPassword(string adminEmail, string targetEmail, string newPassword)
    {
        EnsureUserIsAdmin(adminEmail);

        var targetUser = GetUserByEmailOrThrow(targetEmail);
        targetUser.SetPassword(newPassword);

        _auditService.Log(adminEmail, "Password reset",
            $"Password of {targetEmail} was reset by {adminEmail}");
    }
    
    private User GetUserByEmailOrThrow(string email) =>
        FindByEmail(email) ?? throw new ArgumentException(UserNotFoundErrorMessage);

    private User? FindByEmail(string email) =>
        _repository.GetAll()
            .FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

    private void ValidateEmailIsNotTakenByAnotherUser(string currentEmail, string newEmail)
    {
        bool isChangingEmail = !currentEmail.Equals(newEmail, StringComparison.OrdinalIgnoreCase);
        bool newEmailAlreadyExists = FindByEmail(newEmail) is not null;

        if (isChangingEmail && newEmailAlreadyExists)
            throw new ArgumentException(EmailAlreadyInUseErrorMessage);
    }

    private void EnsureUserIsAdmin(string adminEmail)
    {
        var user = GetUserByEmailOrThrow(adminEmail);

        if (!user.HasRole(Role.Admin))
            throw new ArgumentException(UserNotAuthorizedErrorMessage);
    }
}