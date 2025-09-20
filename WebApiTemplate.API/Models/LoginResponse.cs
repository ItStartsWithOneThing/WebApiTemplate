namespace WebApiTemplate.API.Models;

internal class LoginResponse
{
    public string? SecurityToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
}