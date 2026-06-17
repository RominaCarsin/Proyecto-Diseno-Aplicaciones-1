using DTOs;
using IServices;

namespace Services;

public class SessionService : ISessionService
{
    private readonly IUserService _userService;
    private readonly IAuthStateService _authState;
    private UserResponse? _currentUser;

    public SessionService(IUserService userService, IAuthStateService authState)
    {
        _userService = userService;
        _authState = authState;
    }

    public void Login(string email, string password)
    {
        _currentUser = _userService.Login(email, password);
        _authState.NotifyStateChanged();
    }

    public void Logout()
    {
        _currentUser = null;
        _authState.NotifyStateChanged();
    }

    public bool IsLoggedIn() => _currentUser is not null;

    public string? GetCurrentUserEmail() => _currentUser?.Email;

    public UserResponse? GetCurrentUser() => _currentUser;

    public bool CurrentUserHasRole(RoleDto role)
    {
        if (_currentUser is null)
            return false;

        bool userHasRole = (_currentUser.Roles & role) == role;
        return userHasRole;
    }
}