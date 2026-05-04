using Domain;
using IRepository;

namespace Repository;

public class UserRepository : IUserRepository
{
    private readonly List<User> _users = new();

    public void Add(User entity)
    {
        _users.Add(entity);
    }

    public IEnumerable<User> GetAll()
    {
        return _users;
    }

    public void Delete(User entity)
    {
        _users.Remove(entity);
    }

    public User? GetByEmail(string email)
    {
        return _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }

    public bool ExistsByEmail(string email)
    {
        return _users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }
}

