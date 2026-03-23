namespace webappi.Services.Core
{
    public interface IUrlEncryptionService
    {
        string Encrypt(string plainText);
        string? Decrypt(string cipherText);
    }
}
