using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ArdantOffical.Helpers
{
    public class FgcEncrypt
    {
        public static string Base64Encode(string plainText)
        {
            try
            {
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
                return System.Convert.ToBase64String(plainTextBytes);
            }
            catch (Exception ex)
            {
                return string.Empty;
            }

        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        #region AES256 Security
        // Define a constant 256-bit AES encryption key (32 bytes)
        private static readonly byte[] EncryptionKey = new byte[]
        {
        0x01, 0x23, 0x45, 0x67,
        0x89, 0xAB, 0xCD, 0xEF,
        0xFE, 0xDC, 0xBA, 0x98,
        0x76, 0x54, 0x32, 0x10
        };

        public static string AES256Encrypt(string plainText)
        {
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.KeySize = 256; // Use 256-bit key for AES-256
                aesAlg.Key = EncryptionKey;
                aesAlg.Mode = CipherMode.CFB; // Choose the appropriate mode
                aesAlg.Padding = PaddingMode.PKCS7; // Choose the appropriate padding

                aesAlg.GenerateIV(); // Generate a random IV for each encryption

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length); // Write IV to the beginning of the ciphertext
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    byte[] encryptedBytes = msEncrypt.ToArray();
                    return Convert.ToBase64String(encryptedBytes); // Return ciphertext as a Base64-encoded string
                }
            }
        }

        public static string AES256Decrypt(string cipherText)
        {
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                byte[] cipherBytes = Convert.FromBase64String(cipherText); // Convert Base64-encoded ciphertext back to bytes
                byte[] iv = new byte[16]; // Extract the IV from the ciphertext
                Array.Copy(cipherBytes, iv, 16);

                aesAlg.KeySize = 256; // Use 256-bit key for AES-256
                aesAlg.Key = EncryptionKey;
                aesAlg.Mode = CipherMode.CFB; // Choose the appropriate mode
                aesAlg.Padding = PaddingMode.PKCS7; // Choose the appropriate padding

                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherBytes, 16, cipherBytes.Length - 16))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
        #endregion
    }
}
