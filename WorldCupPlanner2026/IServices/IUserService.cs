using DTOs;

namespace IServices;

public interface IUserService
{
    UserResponse AddUser(UserRequest request);
    void UpdateUser(string email, UserRequest request);
    bool UserHasRole(string email, RoleDto roles);
    void AddRoleToUser(string email, RoleDto role);
    void RemoveRoleFromUser(string email, RoleDto role);
    void DeleteUser(string email);
    UserResponse GetUserByEmail(string email);
    IEnumerable<UserResponse> GetUsers();
    UserResponse Login(string email, string password);
    void ResetPassword(string email);
    void ChangePassword(string email, string newPassword);
}