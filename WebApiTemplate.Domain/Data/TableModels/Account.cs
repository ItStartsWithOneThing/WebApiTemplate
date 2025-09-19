using WebApiTemplate.Domain.Data.TableModels.Base;

namespace WebApiTemplate.Domain.Data.TableModels;

public class Account : BaseEntity
{
    public string Username { get; set; } = null!;
    public string PasswordEncrypted { get; set; } = null!;
    public int EncryptionLevelId { get; set; }
    public string PasswordSalt { get; set; } = null!;
    public string EmailAddress { get; set; } = null!;
    public string? PhoneNumber { get; set; } = null!;
    public string? FirstName { get; set; } = null!;
    public string? LastName { get; set; } = null!;

    public ICollection<RefreshToken> RefreshTokens { get; set; }
}