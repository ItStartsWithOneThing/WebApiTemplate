using WebApiTemplate.Domain.Data.TableModels;

namespace WebApiTemplate.Application.Iterfaces;

public interface IAccountService
{
    Task<IEnumerable<Account>> GetAllAccounts();
}