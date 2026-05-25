using IRepository;

namespace Services.Tests;

public class FakeRepository<T> : IRepository<T> where T : class
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
    
    public void Delete(T entity)
    {
        _entities.Remove(entity);
    }
}