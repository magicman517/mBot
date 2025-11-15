using System.Security.Cryptography;
using Bot.Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace Bot.Application.Services;

public class EncryptionService : IEncryptionService
{
    private readonly byte[] _key;
    private const string Separator = "::";
    
    public EncryptionService(IConfiguration configuration)
    {
        // openssl rand -base64 32
        var encryptionKey = configuration["Encryption:Key"];
        
        if (string.IsNullOrEmpty(encryptionKey))
            throw new InvalidOperationException("Encryption key is not configured");
        
        _key = Convert.FromBase64String(encryptionKey);
    }
    
    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText)) return plainText;
        
        using var aes = Aes.Create();
        aes.Key = _key;
        
        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs))
        {
            sw.Write(plainText);
        }
        
        var encryptedBytes = ms.ToArray();
        var encryptedString = Convert.ToBase64String(encryptedBytes);
        var ivString = Convert.ToBase64String(aes.IV);
        
        return $"{encryptedString}{Separator}{ivString}";
    }

    public string Decrypt(string combinedText)
    {
        if (string.IsNullOrEmpty(combinedText)) return combinedText;
        
        var parts = combinedText.Split(Separator);
        if (parts.Length != 2)
            throw new FormatException("Input text is not in the expected format [encrypted]:[iv]");
        
        var cipherText = parts[0];
        var ivText = parts[1];
        
        var cipherBytes = Convert.FromBase64String(cipherText);
        var ivBytes = Convert.FromBase64String(ivText);

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = ivBytes;
        
        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(cipherBytes);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);
        
        return sr.ReadToEnd();
    }
}