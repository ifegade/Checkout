using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Checkout.PaymentGateway.Utils;

public interface IEncryptionServices
{
    #region Methods

    string DecryptString(string text);
    string EncryptString(string text);
    bool IsEncrypted(string text);

    #endregion
}

public class EncryptionServices : IEncryptionServices
{
    private const string EncryptedValuePrefix = "EncryptedValue:";

    private readonly byte[] key = new byte[32] // 32 bytes = 256-bit.
    {
        73, 84, 28, 39, 182, 122, 193, 73, 43, 71, 106, 142, 76, 16, 54, 19, 21, 115, 138, 75, 45, 114, 41, 79, 181,
        196, 40, 148, 154, 81, 173, 56
    };

    public string DecryptString(string text)
    {
        if (string.IsNullOrWhiteSpace(text) || !IsEncrypted(text))
            // There is no need to decrypt null/empty or unencrypted text.
            return text;

        // Parse the vector from the encrypted data.
        var vector = Convert.FromBase64String(text.Split(';')[0].Split(':')[1]);

        // Decrypt and return the plain text.
        return Decrypt(Convert.FromBase64String(text.Split(';')[1]), key, vector);
    }

    public string EncryptString(string text)
    {
        if (string.IsNullOrWhiteSpace(text) || IsEncrypted(text))
            // There is no need to encrypt null/empty or already encrypted text.
            return text;

        // Create a new random vector.
        var vector = GenerateInitializationVector();

        // Encrypt the text.
        var encryptedText = Convert.ToBase64String(Encrypt(text, key, vector));

        // Format and return the encrypted data.
        return EncryptedValuePrefix + Convert.ToBase64String(vector) + ";" + encryptedText;
    }

    public bool IsEncrypted(string text)
    {
        return text.StartsWith(EncryptedValuePrefix, StringComparison.OrdinalIgnoreCase);
    }

    private string Decrypt(byte[] encryptedBytes, byte[] key, byte[] vector)
    {
        using (var aesAlgorithm = Aes.Create())
        using (var decryptor = aesAlgorithm.CreateDecryptor(key, vector))
        using (var memoryStream = new MemoryStream(encryptedBytes))
        using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
        using (var streamReader = new StreamReader(cryptoStream, Encoding.UTF8))
        {
            return streamReader.ReadToEnd();
        }
    }

    private byte[] Encrypt(string plainText, byte[] key, byte[] vector)
    {
        using (var aesAlgorithm = Aes.Create())
        using (var encryptor = aesAlgorithm.CreateEncryptor(key, vector))
        using (var memoryStream = new MemoryStream())
        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
        {
            using (var streamWriter = new StreamWriter(cryptoStream, Encoding.UTF8))
            {
                streamWriter.Write(plainText);
            }

            return memoryStream.ToArray();
        }
    }

    private byte[] GenerateInitializationVector()
    {
        var aesAlgorithm = Aes.Create();
        aesAlgorithm.GenerateIV();

        return aesAlgorithm.IV;
    }
}