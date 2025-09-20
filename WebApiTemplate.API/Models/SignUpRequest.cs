namespace WebApiTemplate.API.Models;

public class SignUpRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string EmailAddress { get; set; }
    public string? PhoneNumber { get; set; } = null!;
    public string? FirstName { get; set; } = null!;
    public string? LastName { get; set; } = null!;
}