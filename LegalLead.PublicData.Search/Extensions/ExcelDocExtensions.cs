using Bogus;
using LegalLead.PublicData.Search.Helpers;
using LegalLead.PublicData.Search.Interfaces;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LegalLead.PublicData.Search.Extensions
{
    internal static class ExcelDocExtensions
    {
        public static void AppendDocumentProperties(this ExcelPackage package, GenExcelFileParameter context)
        {
            var faker = new Faker();
            var lorem = faker.Lorem.Sentence(15).Replace(' ', '.').Substring(0, 24);
            var json = JsonConvert.SerializeObject(context);
            var encoded = TheCryptoEngine.Encrypt(json, lorem, out var vector);
            var prop = package.Workbook.Properties;
            prop.SetCustomPropertyValue(CustomPropetyNames.Admin, IsAccountAdmin());
            prop.SetCustomPropertyValue(CustomPropetyNames.Data, encoded);
            prop.SetCustomPropertyValue(CustomPropetyNames.DataVector, vector);
            prop.SetCustomPropertyValue(CustomPropetyNames.DataPhrase, lorem);
        }

        public static bool UnlockDocument(this string filePath)
        {
            var hasProperties = filePath.HasDocumentProperties();
            if (!hasProperties) return false;
            if (!filePath.HasSecuredData(out var _)) return false;
            var package = ExcelExtensions.CreateExcelPackage(filePath);
            var wbk = package.Workbook;
            var worksheet = wbk.Worksheets[0];
            // protect workbook with password
            wbk.Protection.SetPassword("");
            // protect worksheet with password
            worksheet.Protection.SetPassword("");
            worksheet.Protection.IsProtected = false;
            // hide all rows except header
            var rows = worksheet.Dimension.Rows;
            for (var i = rows; i > 1; i--)
            {
                var row = worksheet.Row(i);
                row.Hidden = false;
            }
            package.SaveAs(new FileInfo(filePath));
            return true;
        }


        private static bool HasDocumentProperties(this string filePath)
        {
            try
            {
                var package = ExcelExtensions.CreateExcelPackage(filePath);
                if (package == null) return false;
                var prop = package.Workbook.Properties;
                var items = new List<string>()
                {
                    CustomPropetyNames.Admin,
                    CustomPropetyNames.Data,
                    CustomPropetyNames.DataVector,
                    CustomPropetyNames.DataPhrase,
                };
                foreach (var item in items)
                {
                    var element = prop.GetCustomPropertyValue(item);
                    if (element == null) return false;
                    var idx = items.IndexOf(item);
                    if (idx == 0 && element is not bool _) return false;
                    if (element is not string _) return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool HasSecuredData(this string filePath, out GenExcelFileParameter excelData)
        {
            excelData = null;
            try
            {
                var package = ExcelExtensions.CreateExcelPackage(filePath);
                if (package == null) return false;
                var prop = package.Workbook.Properties;
                var metadata = new
                {
                    phrase = Convert.ToString(prop.GetCustomPropertyValue(CustomPropetyNames.DataPhrase)),
                    vector = Convert.ToString(prop.GetCustomPropertyValue(CustomPropetyNames.DataVector)),
                    encoded = Convert.ToString(prop.GetCustomPropertyValue(CustomPropetyNames.Data)),
                };
                var decoded = TheCryptoEngine.Decrypt(metadata.encoded, metadata.phrase, metadata.vector);
                var obj = JsonConvert.DeserializeObject<GenExcelFileParameter>(decoded);
                if (obj != null) excelData = obj;
                return obj != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static class TheCryptoEngine
        {
            public static string Encrypt(string input, string key, out string vector)
            {
                byte[] inputArray = Encoding.UTF8.GetBytes(key);
                var key64 = Convert.ToBase64String(inputArray, 0, inputArray.Length);
                return EncryptData(input, key64, out vector);
            }

            public static string Decrypt(string input, string key, string vectorBase64)
            {
                byte[] inputArray = Encoding.UTF8.GetBytes(key);
                var key64 = Convert.ToBase64String(inputArray, 0, inputArray.Length);
                return DecryptData(input, key64, vectorBase64);
            }

            private static string EncryptData(string plainText, string keyBase64, out string vectorBase64)
            {
                using Aes aesAlgorithm = Aes.Create();
                aesAlgorithm.Key = Convert.FromBase64String(keyBase64);
                aesAlgorithm.GenerateIV();

                vectorBase64 = Convert.ToBase64String(aesAlgorithm.IV);
                // Create encryptor object
                ICryptoTransform encryptor = aesAlgorithm.CreateEncryptor();

                byte[] encryptedData;

                //Encryption will be done in a memory stream through a CryptoStream object
                using (System.IO.MemoryStream ms = new())
                {
                    using CryptoStream cs = new(ms, encryptor, CryptoStreamMode.Write);
                    using (System.IO.StreamWriter sw = new(cs))
                    {
                        sw.Write(plainText);
                    }
                    encryptedData = ms.ToArray();
                }

                return Convert.ToBase64String(encryptedData);
            }

            private static string DecryptData(string cipherText, string keyBase64, string vectorBase64)
            {
                using Aes aesAlgorithm = Aes.Create();
                aesAlgorithm.Key = Convert.FromBase64String(keyBase64);
                aesAlgorithm.IV = Convert.FromBase64String(vectorBase64);

                // Create decryptor object
                ICryptoTransform decryptor = aesAlgorithm.CreateDecryptor();

                byte[] cipher = Convert.FromBase64String(cipherText);

                //Decryption will be done in a memory stream through a CryptoStream object
                using MemoryStream ms = new(cipher);
                using CryptoStream cs = new(ms, decryptor, CryptoStreamMode.Read);
                using StreamReader sr = new(cs);
                return sr.ReadToEnd();
            }
        }

        private static class CustomPropetyNames
        {
            private const string Prefix = "legallead.co";
            public static readonly string Admin = $"{Prefix}.admin";
            public static readonly string Data = $"{Prefix}.data";
            public static readonly string DataVector = $"{Data}.code";
            public static readonly string DataPhrase = $"{Data}.phrase";
        }
        private static bool IsAccountAdmin()
        {
            return GetAccountIndexes().Equals("-1");
        }

        private static string GetAccountIndexes()
        {
            var container = SessionPersistenceContainer.GetContainer;
            var instance = container.GetInstance<ISessionPersistance>(ApiHelper.ApiMode);
            return instance.GetAccountPermissions();
        }
    }
}
