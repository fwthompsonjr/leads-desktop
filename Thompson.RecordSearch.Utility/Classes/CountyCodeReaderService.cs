using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Interfaces;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Classes
{
    public class CountyCodeReaderService : ICountyCodeReader
    {
        private readonly IHttpService _httpService;
        private readonly ICountyCodeService _countyCodeService;
        public CountyCodeReaderService(IHttpService http, ICountyCodeService countyCode)
        {
            _httpService = http;
            _countyCodeService = countyCode;
        }

        public string GetCountyCode(int id, string userId = "")
        {
            lock (locker)
            {
                var isExisting = KeyIndexes.TryGetValue(id, out var code);
                if (isExisting) return code;
                var found = _countyCodeService.Find(id);
                if (found == null) return null;
                var lookup = GetRemoteData(found, userId);
                if (string.IsNullOrEmpty(lookup)) return null;
                KeyIndexes.Add(id, lookup);
                return lookup; 
            }
        }

        public string GetCountyCode(string code, string userId = "")
        {
            lock (locker)
            {
                var isExisting = KeyCodes.TryGetValue(code, out var passcode);
                if (isExisting) return passcode;
                var found = _countyCodeService.Find(code);
                if (found == null) return null;
                var lookup = GetRemoteData(found, userId);
                if (string.IsNullOrEmpty(lookup)) return null;
                KeyCodes.Add(code, lookup);
                return lookup; 
            }
        }

        private string GetRemoteData(CountyCodeDto code, string uid = "")
        {
            const string fallback = "default";
            code.Uid = uid;
            var userId = string.IsNullOrEmpty(code.Uid) ? fallback : code.Uid;
            var address = _countyCodeService.GetWebAddress(1);
            using (var client = new HttpClient())
            {
                var response = _httpService
                    .PostAsJsonAsync<object, JsModel>(client, address, new { name = code.Name, userId })
                    .GetAwaiter()
                    .GetResult();
                if (response == null) return null;
                return GetDecodedData(code, response);
            }
        }

        private static string GetDecodedData(CountyCodeDto code, JsModel model)
        {
            try
            {
                if (code == null) return null;
                if (model == null) return null;
                if (string.IsNullOrEmpty(model.Code)) return null;
                return CryptoEngine.Decrypt(model.Code, code.Code, code.Vector);
            }
            catch { return null; }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Sonar Qube",
            "S1144:Unused private types or members should be removed",
            Justification = "Ignoring rule for this private sealed member")]
        

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Sonar Qube",
            "S3459:Unassigned members should be removed",
            Justification = "As POCO class exposing both get/set operations")]
        private class JsCode
        {
            [JsonProperty("name")] public string Name { get; set; }
            [JsonProperty("user")] public string User { get; set; }
            [JsonProperty("code")] public string Code { get; set; }
        }



        private static class CryptoEngine
        {

            public static string Decrypt(string input, string key, string vectorBase64)
            {
                byte[] inputArray = Encoding.UTF8.GetBytes(key);
                var key64 = Convert.ToBase64String(inputArray, 0, inputArray.Length);
                return DecryptData(input, key64, vectorBase64);
            }

            private static string DecryptData(string cipherText, string keyBase64, string vectorBase64)
            {
                using (Aes aesAlgorithm = Aes.Create())
                {
                    aesAlgorithm.Key = Convert.FromBase64String(keyBase64);
                    aesAlgorithm.IV = Convert.FromBase64String(vectorBase64);

                    // Create decryptor object
                    ICryptoTransform decryptor = aesAlgorithm.CreateDecryptor();

                    byte[] cipher = Convert.FromBase64String(cipherText);

                    //Decryption will be done in a memory stream through a CryptoStream object
                    using (MemoryStream ms = new MemoryStream(cipher))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader sr = new StreamReader(cs)) { return sr.ReadToEnd(); }
                        }
                    }
                }
            }
        }

        private static readonly Dictionary<int, string> KeyIndexes = new Dictionary<int, string>();

        private static readonly Dictionary<string, string> KeyCodes = new Dictionary<string, string>();
        private static readonly object locker = new object();
    }
}
