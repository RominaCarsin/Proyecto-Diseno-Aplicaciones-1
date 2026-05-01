using DTOs;

namespace IServices;

public interface IUserService
{
    UserResponse CreateUser(UserRequest request);
    void UpdateUser(string email, UserRequest request);
    void DeleteUser(string email);
    UserResponse GetUserByEmail(string email);
    IEnumerable<UserResponse> GetUsers();
    UserResponse Login(string email, string password);
    void ResetPassword(string email);
    void ChangePassword(string email, string newPassword);
}