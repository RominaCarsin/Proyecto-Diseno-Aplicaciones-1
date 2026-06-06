using System.Reflection;
using IRepository;

namespace Services.Tests;

public class FakeRepository<T> : IRepository<T> where T : class
{
    private readonly List<T> _entities = new();
    private int _nextId = 1;

    private static readonly PropertyInfo? IdProperty =
        typeof(T).GetProperty($"{typeof(T).Name}Id");

    public void Add(T entity)
    {
        if (IdProperty is not null
            && IdProperty.GetValue(entity) is int id
            && id == 0)
        {
            IdProperty.SetValue(entity, _nextId++);
        }

        _entities.Add(entity);
    }

    public IEnumerable<T> GetAll() => _entities;
    public void Delete(T entity) => _entities.Remove(entity);
    public void Update(T entity) { }
}