using Domain;
using DTOs;
using IServices;
using IRepository;
using Services.Mappers;

namespace Services;

public class UserService : IUserService
{
    private readonly IRepository<User> _repository;
    
    public UserService(IRepository<User> repository)
    {
        _repository = repository;
    }
    
    public UserResponse AddUser(UserRequest request)
    {
        if (FindByEmail(request.Email) != null)
            throw new ArgumentException("User already exists");
        var user = UserMapper.ToEntity(request);
        _repository.Add(user);
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
        
        existingUser.Name = request.Name;
        existingUser.Surname = request.Surname;
        existingUser.DateOfBirth = request.DateOfBirth;
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

    public void AddRoleToUser(string email, RoleDto role)
    {
        User existingUser = GetExistingUserOrThrowArgumentException(email);
        existingUser.AddRole(RoleMapper.ToDomain(role));
    }

    public void RemoveRoleFromUser(string email, RoleDto role)
    {
        User existingUser = GetExistingUserOrThrowArgumentException(email);
        existingUser.RemoveRole(RoleMapper.ToDomain(role));
    }

    public void DeleteUser(string email)
    {
        User existingUser = GetExistingUserOrThrowArgumentException(email);
        _repository.Delete(existingUser);
    }

    public UserResponse GetUserByEmail(string email)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<UserResponse> GetUsers()
    {
        throw new NotImplementedException();
    }

    public UserResponse Login(string email, string password)
    {
        throw new NotImplementedException();
    }

    public void ResetPassword(string email)
    {
        throw new NotImplementedException();
    }

    public void ChangePassword(string email, string newPassword)
    {
        throw new NotImplementedException();
    }
}