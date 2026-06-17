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
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IRepository<User> repository, IAuditService auditService, IPasswordHasher passwordHasher)
    {
        _repository = repository;
        _auditService = auditService;
        _passwordHasher = passwordHasher;
    }

    public UserResponse AddUser(UserRequest userRequest)
    {
        if (FindByEmail(userRequest.Email) is not null)
            throw new ArgumentException(UserAlreadyExistsErrorMessage);

        var user = UserMapper.ToEntity(userRequest, _passwordHasher);
        _repository.Add(user);

        _auditService.Log(userRequest.Email, "Usuario creado",
            $"Nuevo usuario registrado: {userRequest.Name} {userRequest.Surname} ({userRequest.Email})");

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

        _repository.Update(existingUser);
        _auditService.Log(email, "Usuario actualizado",
            $"Usuario {email} actualizado: Nombre={userRequest.Name}, Apellido={userRequest.Surname}, " +
            $"Email={userRequest.Email}, FNac={userRequest.DateOfBirth:yyyy-MM-dd}");
    }

    public void DeleteUser(string email)
    {
        var existingUser = GetUserByEmailOrThrow(email);
        _repository.Delete(existingUser);

        _auditService.Log("system", "Usuario eliminado",
            $"Usuario {email} eliminado del sistema.");
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

        if (!user.VerifyPassword(password, _passwordHasher))
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

        _repository.Update(targetUser);
        
        _auditService.Log(adminEmail, "Rol agregado",
            $"Rol {role} otorgado a {targetEmail} por {adminEmail}");
    }

    public void RemoveRoleFromUser(string adminEmail, string targetEmail, RoleDto role)
    {
        EnsureUserIsAdmin(adminEmail);

        var targetUser = GetUserByEmailOrThrow(targetEmail);
        targetUser.RemoveRole(RoleMapper.ToDomain(role));

        _repository.Update(targetUser);
        _auditService.Log(adminEmail, "Rol revocado",
            $"Rol {role} revocado de {targetEmail} por {adminEmail}");
    }

    public void ChangePassword(string email, string currentPassword, string newPassword)
    {
        var user = GetUserByEmailOrThrow(email);

        if (!user.VerifyPassword(currentPassword, _passwordHasher))
            throw new ArgumentException(CurrentPasswordIncorrectErrorMessage);

        user.SetPassword(newPassword, _passwordHasher);

        _repository.Update(user);
        _auditService.Log(email, "Contraseña cambiada",
            $"Usuario {email} cambió su propia contraseña");
    }

    public void ResetPassword(string adminEmail, string targetEmail, string newPassword)
    {
        EnsureUserIsAdmin(adminEmail);

        var targetUser = GetUserByEmailOrThrow(targetEmail);
        targetUser.SetPassword(newPassword, _passwordHasher);

        _repository.Update(targetUser);
        _auditService.Log(adminEmail, "Contraseña restablecida",
            $"Contraseña de {targetEmail} fue restablecida por {adminEmail}");
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
