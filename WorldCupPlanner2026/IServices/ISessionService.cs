using DTOs;

namespace IServices;

public interface ISessionService
{
    void Login(string email, string password);
    void Logout();
    bool IsLoggedIn();
    string? GetCurrentUserEmail();
    UserResponse? GetCurrentUser();
}