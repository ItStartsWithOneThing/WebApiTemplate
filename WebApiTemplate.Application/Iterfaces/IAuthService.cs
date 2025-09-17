using System.IdentityModel.Tokens.Jwt;
using WebApiTemplate.Domain.Data.TableModels;

namespace WebApiTemplate.Application.Iterfaces;

public interface IAuthService
{
    Task<JwtSecurityToken> Login(string username, string password);
    Task<RefreshToken> CreateRefreshToken(string username);
}