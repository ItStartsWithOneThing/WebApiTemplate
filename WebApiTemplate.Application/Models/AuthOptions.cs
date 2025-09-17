namespace WebApiTemplate.Application.Models;

public class AuthOptions
{
    public int RefreshTokenExpiryInMinutes { get; set; }
    public int AuthTokenExpirationMinutes { get; set; }
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public string Key { get; set; } = null!;

    public string[] Hubs { get; set; } = null!;
}