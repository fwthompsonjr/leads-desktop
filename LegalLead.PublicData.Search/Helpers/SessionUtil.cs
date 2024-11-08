using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace LegalLead.PublicData.Search.Helpers
{
    public static class SessionUtil
    {
        private static readonly byte[] key = new byte[8] { 1, 12, 3, 14, 5, 16, 7, 18 };
        private static readonly byte[] iv = new byte[8] { 21, 2, 23, 4, 25, 6, 27, 8 };
        private static readonly Encoding encoding = Encoding.UTF8;
        public static string Crypt(this string text)
        {
            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateEncryptor(key, iv);
            byte[] inputbuffer = encoding.GetBytes(text);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Convert.ToBase64String(outputBuffer);
        }

        public static string Decrypt(this string text)
        {
            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateDecryptor(key, iv);
            byte[] inputbuffer = Convert.FromBase64String(text);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return encoding.GetString(outputBuffer);
        }

        public static void Initialize()
        {
            lock (locker)
            {
                var fileName = SetupFile;
                if (string.IsNullOrEmpty(fileName)) return;
                if (File.Exists(fileName)) File.Delete(fileName);
                File.WriteAllText(fileName, ""); 
            }
        }
        public static string Read()
        {
            lock (locker)
            {
                var fileName = SetupFile;
                if (string.IsNullOrEmpty(fileName)) return null;
                if (!File.Exists(fileName)) return null;
                var content = File.ReadAllText(fileName);
                if (string.IsNullOrWhiteSpace(content)) return null;
                try
                {
                    return content.Decrypt();
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
        private static string SetupFile
        {
            get
            {
                if (setupFileName != null) return setupFileName;
                setupFileName = SetupFileName();
                return setupFileName;
            }
        }

        private static string SetupFileName()
        {
            var setupFolder = SetupDirectoyName();
            if (string.IsNullOrEmpty(setupFolder) ||
                !Directory.Exists(setupFolder)) return string.Empty;
            var setupFile = Path.Combine(setupFolder, "session.dat");
            if (string.IsNullOrEmpty(setupFile) ||
                !File.Exists(setupFile)) return string.Empty;
            return setupFile;
        }
        private static string SetupDirectoyName()
        {
            var appFolder = Path.GetDirectoryName(CurrentAssembly.Location);
            var setupFolder = Path.Combine(appFolder, "_session");
            if (!Directory.Exists(setupFolder)) return string.Empty;
            return setupFolder;
        }
        private static Assembly CurrentAssembly
        {
            get
            {
                if (executingAssembly != null) return executingAssembly;
                executingAssembly = Assembly.GetExecutingAssembly();
                return executingAssembly;
            }
        }
        private static string setupFileName = null;
        private static Assembly executingAssembly = null;
        private static readonly object locker = new();
    }
}