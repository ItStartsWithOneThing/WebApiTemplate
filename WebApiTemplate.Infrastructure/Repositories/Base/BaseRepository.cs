using WebApiTemplate.Domain.Data.Interfaces.Base;
using WebApiTemplate.Domain.Data.TableModels.Base;

namespace WebApiTemplate.Infrastructure.Repositories.Base;

public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
{
    private readonly MockDatabase database = MockDatabase.Instance;

    // Better to use exact methods/specifications istead of using predicates in order to encapsulate logic

    // For in-memory data use 'Func<T,bool>'
    // For external data use 'Expression<Func<T,bool>>'

    public Task Add(T entity)
    {
        database.Add(entity);

        return Task.CompletedTask;
    }
    public Task Update(Func<T, bool> predicate, Action<T> updateAction)
    {
        database.Update(predicate, updateAction);

        return Task.CompletedTask;
    }

    public Task Remove(Predicate<T> predicate)
    {
        database.Remove(predicate);

        return Task.CompletedTask;
    }

    public Task<IEnumerable<T>> Get(Func<T, bool>? predicate = null)
    {
        var result = database.GetAll<T>();

        if(predicate == null)
        {
            return Task.FromResult(result.AsEnumerable());
        }

        return Task.FromResult(result.Where(predicate).AsEnumerable());
    }

    public Task<T?> GetFirstOrDefault(Func<T, bool> predicate)
    {
        var result = database.GetAll<T>().FirstOrDefault(predicate);

        return Task.FromResult(result);
    }
}