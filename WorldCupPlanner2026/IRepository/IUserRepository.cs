using Domain;

namespace IRepository;

public interface IUserRepository : IRepository<User>
{
    User? GetByEmail(string email);
    bool ExistsByEmail(string email);
}
