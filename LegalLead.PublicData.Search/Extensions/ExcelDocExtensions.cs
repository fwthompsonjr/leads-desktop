using Bogus;
using LegalLead.PublicData.Search.Helpers;
using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Util;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Models;

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
            var wb = package.Workbook;
            var prop = wb.Properties;
            context.WriteProperties(wb);
            prop.SetCustomPropertyValue(CustomPropetyNames.Admin, TheUserManager.IsAccountAdmin());
            prop.SetCustomPropertyValue(CustomPropetyNames.UserName, TheUserManager.GetUserName());
            prop.SetCustomPropertyValue(CustomPropetyNames.Data, encoded);
            prop.SetCustomPropertyValue(CustomPropetyNames.DataVector, vector);
            prop.SetCustomPropertyValue(CustomPropetyNames.DataPhrase, lorem);
        }


        public static bool HasUserNameMatch(this string filePath)
        {
            lock (locker)
            {
                var isAdmin = TheUserManager.IsAccountAdmin();
                if (isAdmin) return true;
                var hasProperties = filePath.HasDocumentProperties();
                if (!hasProperties) return false;
                var package = ExcelExtensions.CreateExcelPackage(filePath);
                var prop = package.Workbook.Properties;
                var currentUser = TheUserManager.GetUserName();
                var element = prop.GetCustomPropertyValue(CustomPropetyNames.UserName);
                if (element is not string actual) return false;
                return currentUser.Equals(actual); 
            }
        }

        public static bool UnlockDocument(this string filePath)
        {
            var isAdmin = TheUserManager.IsAccountAdmin();
            if (isAdmin) return true;
            var hasProperties = filePath.HasDocumentProperties();
            if (!hasProperties) return false;
            if (!filePath.HasUserNameMatch()) return false;
            if (!filePath.HasSecuredData(out var excelProperties)) return false;
            var package = ExcelExtensions.CreateExcelPackage(filePath);
            var wbk = package.Workbook;
            var worksheet = wbk.Worksheets[0];
            if (!worksheet.Protection.IsProtected) return true;
            if (string.IsNullOrEmpty(excelProperties?.TrackingIndex)) return false;
            var invoiceData = invoiceReader.GetInvoicesByTrackingId(TheUserManager.GetAccountId(), excelProperties.TrackingIndex);
            var model = invoiceData.ToInstance<GetInvoiceResponse>();
            if (model == null || model.Headers.Count == 0) return false;
            var invoice = model.Headers[0];
            if (!invoice.InvoiceNbr.Equals("PAID")) return false;
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
            using var ms = new MemoryStream();
            package.SaveAs(ms);
            var data = ms.ToArray();
            if (File.Exists(filePath)) File.Delete(filePath);
            File.WriteAllBytes(filePath, data);
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
                    CustomPropetyNames.UserName,
                };
                foreach (var item in items)
                {
                    var element = prop.GetCustomPropertyValue(item);
                    if (element == null) return false;
                    var idx = items.IndexOf(item);
                    if (idx == 0 && element is not bool _) return false;
                    if (idx > 0 && element is not string _) return false;
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
                var culture = CultureInfo.CurrentCulture;
                var prop = package.Workbook.Properties;
                var metadata = new
                {
                    phrase = Convert.ToString(prop.GetCustomPropertyValue(CustomPropetyNames.DataPhrase), culture),
                    vector = Convert.ToString(prop.GetCustomPropertyValue(CustomPropetyNames.DataVector), culture),
                    encoded = Convert.ToString(prop.GetCustomPropertyValue(CustomPropetyNames.Data), culture),
                };
                var hasMetaData = !string.IsNullOrEmpty(metadata.phrase) && 
                    !string.IsNullOrEmpty(metadata.vector) &&
                    !string.IsNullOrEmpty(metadata.encoded);
                if (!hasMetaData) return false;
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

        private static void WriteProperties(this GenExcelFileParameter dto, ExcelWorkbook wb)
        {
            var description = dto.Description();
            var props = new Dictionary<string, string>() {
                { "Created", DateTime.UtcNow.ToString("o") },
                { "Comments", description ?? $"{CustomPropetyNames.Prefix} Record Search" }
            };

            var keys = CommonProperties.Keys.ToList();
            keys.ForEach(x =>
            {
                var kvalue = CommonProperties[x];
                if (props.ContainsKey(x)) { kvalue = props[x]; }
                switch (x)
                {
                    case "Author":
                        wb.Properties.Author = kvalue;
                        wb.Properties.LastModifiedBy = kvalue;
                        wb.Properties.Subject = GetWbSubject(description, $"{dto.RecordCount}");
                        break;
                    case "Title":
                        wb.Properties.Title = kvalue;
                        break;
                    case "Comments":
                        wb.Properties.Comments = kvalue;
                        break;
                    case "Company":
                        wb.Properties.Company = kvalue;
                        break;
                    case "Created":
                        wb.Properties.Created = DateTime.UtcNow;
                        break;
                }
            });
        }
        private static string GetWbSubject(string description, string rowcount)
        {
            const string fallbak = "Data inquiry";
            if (description == null) return fallbak;
            if (!description.Contains('-')) return fallbak;
            if (!description.Contains(':')) return fallbak;
            var collection = description.Split('-');
            var countyName = collection[0].Split(':')[1].Trim();
            return $"{countyName}, Records: {rowcount}";
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
            internal const string Prefix = "legallead.co";
            public static readonly string Admin = $"{Prefix}.admin";
            public static readonly string Data = $"{Prefix}.data";
            public static readonly string DataVector = $"{Data}.code";
            public static readonly string DataPhrase = $"{Data}.phrase";
            public static readonly string UserName = $"{Prefix}.user.name";
        }

        private static class TheUserManager
        {
            public static bool IsAccountAdmin()
            {
                return GetAccountIndexes().Equals("-1");
            }
            public static string GetAccountId()
            {
                return userReader.GetAccountId();
            }

            public static string GetUserName()
            {
                return userReader.GetUserName();
            }

            private static string GetAccountIndexes()
            {
                return userReader.GetAccountPermissions();
            }

            private static readonly ISessionPersistance userReader
                = SessionPersistenceContainer.GetContainer.GetInstance<ISessionPersistance>(ApiHelper.ApiMode);
        }

        private static readonly Dictionary<string, string> CommonProperties = new() {
        { "Author", CustomPropetyNames.Prefix },
        { "Title", $"{CustomPropetyNames.Prefix} Record Search" },
        { "Created", "" },
        { "Comments", "" },
        { "Company", CustomPropetyNames.Prefix },
        };

        private static readonly IRemoteInvoiceHelper invoiceReader = ActionSettingContainer
        .GetContainer
        .GetInstance<IRemoteInvoiceHelper>();
        private static readonly object locker = new object();
    }
}
