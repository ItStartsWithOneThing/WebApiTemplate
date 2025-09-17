namespace WebApiTemplate.Domain.Data.TableModels;

public class RefreshToken
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public string? Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsActive { get; set; }
}