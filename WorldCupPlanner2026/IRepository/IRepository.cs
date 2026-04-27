namespace IRepository;

public  interface IRepository<T>
{
    void Add(T entity);
    IEnumerable<T> GetAll();
    void Delete(T entity);
}