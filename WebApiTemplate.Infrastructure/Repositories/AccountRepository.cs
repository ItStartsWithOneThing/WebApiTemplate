using WebApiTemplate.Domain.Data.Interfaces;
using WebApiTemplate.Domain.Data.TableModels;
using WebApiTemplate.Infrastructure.Repositories.Base;

namespace WebApiTemplate.Infrastructure.Repositories;

public class AccountRepository : BaseRepository<Account>, IAccountRepository
{
}