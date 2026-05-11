using Domain;
using DTOs;
using IServices;
using IRepository;
using Services.Mappers;

namespace Services;

public class UserService : IUserService
{
    private readonly IRepository<User> _repository;
    private readonly IAuditService _auditService;
    private readonly ISessionService _sessionService;
    
    public UserService(IRepository<User> repository,  IAuditService auditService, ISessionService sessionService)
    {
        _repository = repository;
        _auditService = auditService;
        _sessionService = sessionService;
    }
    
    public UserResponse AddUser(UserRequest request)
    {
        if (FindByEmail(request.Email) != null)
            throw new ArgumentException("User already exists");
        var user = UserMapper.ToEntity(request);
        _repository.Add(user);
        _auditService.Log(request.Email, "User created", $"New user registered: {request.Name} {request.Surname} ({request.Email})");
        return UserMapper.FromEntity(user);
    }

    private User GetExistingUserOrThrowArgumentException(string email) =>
        FindByEmail(email) ?? throw new ArgumentException("User not found.");
    
    private User? FindByEmail(string email) =>
        _repository.GetAll().FirstOrDefault(u =>  u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    
    public void UpdateUser(string email, UserRequest request)
    {
        User existingUser = GetExistingUserOrThrowArgumentException(email);
        ValidateEmailNotInUse(email, request.Email);
        existingUser.UpdatePersonalInfo(
            request.Name,
            request.Surname,
            request.Email,
            request.DateOfBirth
        );
        _auditService.Log(email, "User updated", $"[USER UPDATE] Target: {email} | " +
                                                 $"New Data -> Name: {request.Name}, Surname: {request.Surname}, " +
                                                 $"Email: {request.Email}, DOB: {request.DateOfBirth:yyyy-MM-dd}");
    }

    private void ValidateEmailNotInUse(string currentEmail, string newEmail)
    {
        if (!currentEmail.Equals(newEmail, StringComparison.OrdinalIgnoreCase)
            && FindByEmail(newEmail) != null)
        {
            throw new ArgumentException("Email already in use");
        }
    }

    public bool UserHasRole(string email, RoleDto role)
    {
        return GetExistingUserOrThrowArgumentException(email)
            .HasRole(RoleMapper.ToDomain(role));
    }

    public void AddRoleToUser(string adminEmail, string targetEmail, RoleDto role)
    {
        EnsureAdmin(adminEmail);
        var user = GetExistingUserOrThrowArgumentException(targetEmail);
        user.AddRole(RoleMapper.ToDomain(role));
        _auditService.Log(adminEmail, "Role added", $"Role {role} granted to {targetEmail} by {adminEmail}");
    }

    public void RemoveRoleFromUser(string adminEmail, string targetEmail, RoleDto role)
    {
        EnsureAdmin(adminEmail);

        var user = GetExistingUserOrThrowArgumentException(targetEmail);
        user.RemoveRole(RoleMapper.ToDomain(role));
        _auditService.Log(adminEmail, "Role removed", $"Role {role} revoked from {targetEmail} by {adminEmail}");
    }

    public void DeleteUser(string email)
    {
        User existingUser = GetExistingUserOrThrowArgumentException(email);
        _repository.Delete(existingUser);
        _auditService.Log(_sessionService.GetCurrentUserEmail() ?? "system", "User deleted", $"User {email} was removed from the system");
    }

    public UserResponse GetUserByEmail(string email)
    {
        User existingUser = GetExistingUserOrThrowArgumentException(email);
        return UserMapper.FromEntity(existingUser);
    }

    public IEnumerable<UserResponse> GetUsers()
    {
        return _repository.GetAll().Select(UserMapper.FromEntity);
    }

    public UserResponse Login(string email, string password)
    {
        User existingUser = GetExistingUserOrThrowArgumentException(email);
        if (!existingUser.VerifyPassword(password))
            throw new ArgumentException("Invalid password");
        return UserMapper.FromEntity(existingUser);
    }

    public void ResetPassword(string adminEmail, string targetEmail, string newPassword)
    {
        EnsureAdmin(adminEmail);

        var targetUser = GetExistingUserOrThrowArgumentException(targetEmail);

        targetUser.SetPassword(newPassword);
        _auditService.Log(adminEmail, "Password reset", $"Password of {targetEmail} was reset by {adminEmail}");
    }

    private void EnsureAdmin(string adminEmail)
    {
        var admin = GetExistingUserOrThrowArgumentException(adminEmail);

        if (!admin.HasRole(Role.Admin))
            throw new ArgumentException("User is not authorized");
    }

    public void ChangePassword(string email, string currentPassword, string newPassword)
    {
        var user = GetExistingUserOrThrowArgumentException(email);

        if (!user.VerifyPassword(currentPassword))
            throw new ArgumentException("Current password is incorrect");

        user.SetPassword(newPassword);
        _auditService.Log(email, "Password changed", $"User {email} changed their own password");
    }
}