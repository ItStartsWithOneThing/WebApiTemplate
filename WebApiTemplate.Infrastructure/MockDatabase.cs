using WebApiTemplate.Domain.Data.TableModels;
using WebApiTemplate.Domain.Data.TableModels.Base;

namespace WebApiTemplate.Infrastructure;

internal sealed class MockDatabase
{
    private static MockDatabase? _instance;
    private static readonly object _instanceLock = new();

    // Per-instance lock for operations
    private readonly object _lock = new();

    // Storage: Type -> object (boxed list of T)
    private readonly Dictionary<Type, object> _tables = new() {
        {
            typeof(Account),
            new List<Account>()
            {
                new Account()
                {
                    Id = 1,
                    Username = "tom1",
                    PasswordEncrypted = "iSlsvINMgG7SeN55k5kULGqkGkmUpuLlvHycG4KTduY=", // abc
                    EncryptionLevelId = 2,
                    PasswordSalt = "di4ZUfo/dCgoP91Gpoh/g5VAv79zIhwvfg+tQz27vmbYr8VVrDYeztgQQIURRlEwYqYG2Gl0E2Sr/RSwJpBsa2vE4jCId8fQOC8oVjw=",
                    EmailAddress = "tom@email.com",
                    PhoneNumber = "",
                    FirstName = "",
                    LastName = "",
                    RefreshTokens = []
                }
            }
        }
    };

    private MockDatabase() { }

    public static MockDatabase Instance
    {
        get
        {
            if (_instance is null)
            {
                lock (_instanceLock)
                {
                    if (_instance is null)
                        _instance = new MockDatabase();
                }
            }
            return _instance;
        }
    }

    public List<T> GetAll<T>() where T: BaseEntity
    {
        lock (_lock)
        {
            var list = GetOrCreateList<T>();

            return list.ToList(); // Returns copy in order to not be able to modify collection without API
        }
    }

    public void Add<T>(T item) where T : BaseEntity
    {
        if (item is null) throw new ArgumentNullException(nameof(item));
        lock (_lock)
        {
            var id = GetAll<T>().Count + 1;
            item.Id = id;

            var list = GetOrCreateList<T>();
            list.Add(item);
        }
    }

    public bool Remove<T>(Predicate<T> predicate) where T : BaseEntity
    {
        if (predicate is null) throw new ArgumentNullException(nameof(predicate));
        lock (_lock)
        {
            var list = GetOrCreateList<T>();
            var toRemove = list.FirstOrDefault(x => predicate(x));

            if (toRemove is null) return false;

            return list.Remove(toRemove);
        }
    }

    public bool Update<T>(Func<T, bool> predicate, Action<T> updateAction) where T : BaseEntity
    {
        if (predicate is null) throw new ArgumentNullException(nameof(predicate));
        if (updateAction is null) throw new ArgumentNullException(nameof(updateAction));

        lock (_lock)
        {
            var list = GetOrCreateList<T>();
            var item = list.FirstOrDefault(x => predicate(x));

            if (item is null) return false;

            updateAction(item);

            return true;
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            _tables.Clear();
        }
    }

    private List<T> GetOrCreateList<T>() where T : BaseEntity
    {
        var type = typeof(T);
        if (!_tables.TryGetValue(type, out var list))
        {
            var newList = new List<T>();
            _tables[type] = newList;

            return newList;
        }

        return (List<T>)list;
    }
}