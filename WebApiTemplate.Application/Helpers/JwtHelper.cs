using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApiTemplate.Application.Constants;
using WebApiTemplate.Application.Models;

namespace WebApiTemplate.Application.Helpers;

internal static class JwtHelper
{
    internal static string IssueRefreshToken(int size = AuthConstants.Default_Crypto_Key_Size)
    {
        var randomBytes = new byte[size];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }

        var token = Convert.ToBase64String(randomBytes)
            .Replace("+", "")
            .Replace("/", "")
            .Replace("=", "");

        return token;
    }

    internal static JwtSecurityToken IssueJwtToken(IEnumerable<Claim> claims, AuthOptions authOptions)
    {
        var token = new JwtSecurityToken(
            issuer: authOptions.Issuer,
            audience: authOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(authOptions.AuthTokenExpirationMinutes),
            notBefore: DateTime.UtcNow,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions.Key)),
                SecurityAlgorithms.HmacSha256)
        );

        return token;
    }
}