using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebApiTemplate.Application.Helpers;
using WebApiTemplate.Application.Iterfaces;
using WebApiTemplate.Application.Models;
using WebApiTemplate.Domain.Data.Interfaces;
using WebApiTemplate.Domain.Data.TableModels;

namespace WebApiTemplate.Application.Services;

internal class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _logger;
    private readonly AuthOptions _authOptions;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IAccountRepository _accountRepository;

    public AuthService(
        ILogger<AuthService> logger,
        IOptions<AuthOptions> authOptions,
        IRefreshTokenRepository refreshTokenRepository,
        IAccountRepository accountRepository)
    {
        _logger = logger;
        _authOptions = authOptions.Value;
        _refreshTokenRepository = refreshTokenRepository;
        _accountRepository = accountRepository;
    }

    public async Task<JwtSecurityToken> Login(string username, string password)
    {
        var account = await GetAccount(username);
        if (account == null)
        {
            _logger.LogTrace("Account with login: '{username}' not found", username);
            throw new UnauthorizedAccessException();
        }

        if(!AuthHelper.IsPasswordValid(account.PasswordEncrypted, account.PasswordSalt, account.EncryptionLevelId, password))
        {
            _logger.LogTrace("Wrong password");
            throw new UnauthorizedAccessException();
        }

        var securityToken = CreateJwtToken(account);

        if (securityToken == null) 
        {
            throw new UnauthorizedAccessException();
        }

        return securityToken;
    }

    public async Task<RefreshToken> CreateRefreshToken(string username)
    {
        var account = await GetAccount(username);
        if (account == null)
        {
            _logger.LogTrace("Account with login: '{username}' not found", username);
            throw new UnauthorizedAccessException();
        }

        var newRefreshToken = new RefreshToken
        {
            AccountId = account.Id,
            Token = JwtHelper.IssueRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_authOptions.RefreshTokenExpiryInMinutes)),
            IsActive = true
        };

        await _refreshTokenRepository.Add(newRefreshToken);

        _logger.LogTrace("Refresh token was added to user: '{login}'", account.Username);

        return newRefreshToken;
    }

    private async Task<Account?> GetAccount(string username)
    {
        return await _accountRepository.GetFirstOrDefault(x => x.Username == username);
    }

    private JwtSecurityToken? CreateJwtToken(Account account)
    {
        var claims = GetAuthClaims(account);
        var securityToken = JwtHelper.IssueJwtToken(claims, _authOptions);

        _logger.LogTrace("Token for user: '{username}' was created ane expires at: '{expires}'", account.Username, securityToken.ValidTo);

        return securityToken;
    }

    private static List<Claim> GetAuthClaims(Account account)
    {
        return 
        [
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, account.Username),
            new(JwtClaimNames.AccountId, account.Id.ToString()),
            new(JwtClaimNames.FirstName, account.FirstName?.ToString() ?? string.Empty),
            new(JwtClaimNames.LastName, account.LastName?.ToString() ?? string.Empty)
        ];
    }
}

file static class JwtClaimNames
{
    public const string AccountId = "accountId";
    public const string FirstName = "firstName";
    public const string LastName = "lastName";
}