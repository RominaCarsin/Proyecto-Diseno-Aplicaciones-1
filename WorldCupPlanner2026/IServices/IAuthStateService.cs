namespace IServices;

public interface IAuthStateService
{
    event Action? OnChange;
    void NotifyStateChanged();
}