using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace webappi.Services.Core
{
    public class UrlEncryptionService : IUrlEncryptionService
    {
        private readonly IDataProtector _protector;

        public UrlEncryptionService(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("UrlEncryptionService.v1");
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;

            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var protectedBytes = _protector.Protect(plainBytes);
            return WebEncoders.Base64UrlEncode(protectedBytes);
        }

        public string? Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return null;

            try
            {
                var protectedBytes = WebEncoders.Base64UrlDecode(cipherText);
                var plainBytes = _protector.Unprotect(protectedBytes);
                return Encoding.UTF8.GetString(plainBytes);
            }
            catch
            {

                return null;
            }
        }
    }
}
