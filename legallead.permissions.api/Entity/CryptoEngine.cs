using System.Security.Cryptography;
using System.Text;

namespace legallead.permissions.api.Entity
{
    internal static class CryptoEngine
    {
        public static string Encrypt(string input, string key)
        {
            byte[] inputArray = Encoding.UTF8.GetBytes(input);
            using var provider = Aes.Create();
            var bytes = Encoding.UTF8.GetBytes(key).Take(32).ToArray();
            provider.Key = bytes;
            provider.Mode = CipherMode.ECB;
            provider.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = provider.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            provider.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decrypt(string input, string key)
        {
            byte[] inputArray = Convert.FromBase64String(input);
            using var provider = Aes.Create();
            var bytes = Encoding.UTF8.GetBytes(key).Take(32).ToArray();
            provider.Key = bytes;
            provider.Mode = CipherMode.ECB;
            provider.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = provider.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            provider.Clear();
            return Encoding.UTF8.GetString(resultArray);
        }
    }

}
