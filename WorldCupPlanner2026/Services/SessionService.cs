using DTOs;
using IServices;

namespace Services;

public class SessionService : ISessionService
{
    private readonly IUserService _userService;
    private UserResponse? _currentUser;

    public SessionService(IUserService userService)
    {
        _userService = userService;
    }

    public void Login(string email, string password)
    {
        _currentUser = _userService.Login(email, password);
    }

    public void Logout()
    {
        _currentUser = null;
    }

    public bool IsLoggedIn() => _currentUser != null;

    public string? GetCurrentUserEmail() => _currentUser?.Email;

    public UserResponse? GetCurrentUser() => _currentUser;
}