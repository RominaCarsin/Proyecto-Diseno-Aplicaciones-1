using IRepository;

namespace Repository;

public class Repository<T> : IRepository<T>
{
    private readonly List<T> _entities = new();

    public void Add(T entity)
    {
        _entities.Add(entity);
    }

    public IEnumerable<T> GetAll()
    {
        return _entities;
    }
}