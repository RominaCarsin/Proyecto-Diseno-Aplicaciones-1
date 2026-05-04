using DTOs;

namespace IServices;

public interface IUserService
{
    UserResponse AddUser(UserRequest request);
    void UpdateUser(string email, UserRequest request);
    bool UserHasRole(string email, RoleDto roles);
    void AddRoleToUser(string adminEmail, string targetEmail, RoleDto role);
    void RemoveRoleFromUser(string adminEmail, string targetEmail, RoleDto role);
    void DeleteUser(string email);
    UserResponse GetUserByEmail(string email);
    IEnumerable<UserResponse> GetUsers();
    UserResponse Login(string email, string password);
    void ChangePassword(string email, string currentPassword, string newPassword);
    void ResetPassword(string adminEmail, string targetEmail, string newPassword);
}