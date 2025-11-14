using System.Security.Cryptography;
using Bot.Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace Bot.Application.Services;

public class EncryptionService : IEncryptionService
{
    private readonly byte[] _key;
    private readonly byte[] _iv;
    
    public EncryptionService(IConfiguration configuration)
    {
        // openssl rand -base64 32
        var encryptionKey = configuration["Encryption:Key"];
        
        // openssl rand -base64 16
        var iv = configuration["Encryption:IV"];
        
        if (string.IsNullOrEmpty(encryptionKey) || string.IsNullOrEmpty(iv))
            throw new InvalidOperationException("Encryption is misconfigured");
        
        _key = Convert.FromBase64String(encryptionKey);
        _iv = Convert.FromBase64String(iv);
    }
    
    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText)) return plainText;

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;
        
        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        using var sw = new StreamWriter(cs);
        
        sw.Write(plainText);
        sw.Flush();
        cs.FlushFinalBlock();
        
        return Convert.ToBase64String(ms.ToArray());
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText)) return cipherText;

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;
        
        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);
        
        return sr.ReadToEnd();
    }
}