using Domain;
using DTOs;
using IServices;
using IRepository;
using Services.Mappers;

namespace Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public UserResponse AddUser(UserRequest request)
    {
        if (FindByEmail(request.Email) != null)
            throw new ArgumentException("User already exists");
        var user = UserMapper.ToEntity(request);
        _userRepository.Add(user);
        return UserMapper.FromEntity(user);
    }

    private User GetExistingUserOrThrowArgumentException(string email) =>
        FindByEmail(email) ?? throw new ArgumentException("User not found.");
    
    private User? FindByEmail(string email)
    {
        return _userRepository.GetByEmail(email);
    }
    
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
    }

    private void ValidateEmailNotInUse(string currentEmail, string newEmail)
    {
        if (!currentEmail.Equals(newEmail, StringComparison.OrdinalIgnoreCase)
            && _userRepository.ExistsByEmail(newEmail))
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
    }

    public void RemoveRoleFromUser(string adminEmail, string targetEmail, RoleDto role)
    {
        EnsureAdmin(adminEmail);

        var user = GetExistingUserOrThrowArgumentException(targetEmail);
        user.RemoveRole(RoleMapper.ToDomain(role));
    }

    public void DeleteUser(string email)
    {
        User existingUser = GetExistingUserOrThrowArgumentException(email);
        _userRepository.Delete(existingUser);
    }

    public UserResponse GetUserByEmail(string email)
    {
        User existingUser = GetExistingUserOrThrowArgumentException(email);
        return UserMapper.FromEntity(existingUser);
    }

    public IEnumerable<UserResponse> GetUsers()
    {
        return _userRepository.GetAll().Select(UserMapper.FromEntity);
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
    }
}