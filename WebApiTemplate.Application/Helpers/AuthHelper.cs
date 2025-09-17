using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace WebApiTemplate.Application.Helpers;

internal static class AuthHelper
{
    internal static bool IsPasswordValid(
        string passwordEncrypted,
        string passwordSalt,
        int encryptionLevelId,
        string password)
    {
        return passwordEncrypted == CreatePasswordHash(password, passwordSalt, encryptionLevelId);
    }

    private static string? CreatePasswordHash(
        string? password,
        string? salt,
        int? encryptionLevelId)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(salt) || encryptionLevelId == null)
        {
            return null;
        }

        password = RemoveDiacritics(password);

        var algorithmName = encryptionLevelId == 1 ? "SHA-1" : "SHA-256";
        var algorithm = HashAlgorithm.Create(algorithmName);

        var saltAndPassword = string.Concat(password ?? string.Empty, salt ?? string.Empty);
        var hash = algorithm?.ComputeHash(Encoding.UTF8.GetBytes(saltAndPassword));
        return Convert.ToBase64String(hash!);
    }

    private static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            UnicodeCategory[] nonPrintableChar = 
            { 
                UnicodeCategory.NonSpacingMark,
                UnicodeCategory.Control,
                UnicodeCategory.OtherNotAssigned
            };

            if (!nonPrintableChar.Contains(unicodeCategory))
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}