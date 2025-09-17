namespace WebApiTemplate.Domain.Data.Interfaces.Base;

public interface IBaseRepository<T>
{
    Task Add(T entity);
    Task Update(Func<T, bool> predicate, Action<T> updateAction);
    Task Remove(Predicate<T> predicate);
    Task<IEnumerable<T>> Get(Func<T, bool>? predicate = null);
    Task<T?> GetFirstOrDefault(Func<T, bool> predicate);
}