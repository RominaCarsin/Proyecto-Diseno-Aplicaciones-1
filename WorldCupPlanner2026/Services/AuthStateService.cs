using IServices;

namespace Services;

public class AuthStateService : IAuthStateService
{
    public event Action? OnChange;
    public void NotifyStateChanged() => OnChange?.Invoke();
}