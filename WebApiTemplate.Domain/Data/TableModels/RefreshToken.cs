using WebApiTemplate.Domain.Data.TableModels.Base;

namespace WebApiTemplate.Domain.Data.TableModels;

public class RefreshToken : BaseEntity
{
    public int AccountId { get; set; }
    public string? Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsActive { get; set; }
}