using System.IdentityModel.Tokens.Jwt;
using WebApiTemplate.Application.Models;
using WebApiTemplate.Domain.Data.TableModels;

namespace WebApiTemplate.Application.Iterfaces;

public interface IAuthService
{
    Task RegisterUser(SignUpInfo signUpInfo);
    Task<JwtSecurityToken> Login(string username, string password);
    Task<RefreshToken> CreateRefreshToken(string username);
}