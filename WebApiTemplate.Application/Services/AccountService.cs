using WebApiTemplate.Application.Iterfaces;
using WebApiTemplate.Domain.Data.Interfaces;
using WebApiTemplate.Domain.Data.TableModels;

namespace WebApiTemplate.Application.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;

    public AccountService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public Task<IEnumerable<Account>> GetAllAccounts()
    {
        return _accountRepository.Get();
    }
}