using LegalLead.PublicData.Search.Common;
using LegalLead.PublicData.Search.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Thompson.RecordSearch.Utility.Dto;

namespace LegalLead.PublicData.Search.Helpers
{
    public static class SessionUtil
    {
        private const string instanceName = ApiHelper.ApiMode;
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
            GetPersistence().Initialize();
        }
        public static List<SettingMenuModel> GetMenuOptions => menuoptions;
        public static string GetCountyAccountName(string county = "")
        {
            return GetPersistence().GetAccountCredential(county);
        }
        public static string Read()
        {
            return GetPersistence().Read();
        }
        public static bool Write(string content)
        {
            return GetPersistence().Write(content);
        }
        private static ISessionPersistance GetPersistence()
        {
            var container = SessionPersistenceContainer.GetContainer;
            var implementation = container.GetInstance<ISessionPersistance>(instanceName);
            return implementation;
        }

        internal static string GetFullFileName(string name)
        {
            var setupFolder = SetupDirectoyName();
            if (string.IsNullOrEmpty(setupFolder) ||
                !Directory.Exists(setupFolder)) return string.Empty;
            var setupFile = Path.Combine(setupFolder, name);
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

        internal static PermissionMapDto GetPermissionsMap(string details)
        {
            try
            {
                if (string.IsNullOrEmpty(details)) return null;
                return JsonConvert.DeserializeObject<PermissionMapDto>(details);
            }
            catch (Exception)
            {

                return null;
            }
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
        private static Assembly executingAssembly = null;
        private static readonly List<SettingMenuModel> menuoptions
            = new()
            {
                new () { Id = 0, Name = "Change Password" },
                new () { Id = 1, Name = "County Permissions" },
                new () { Id = 2, Name = "User Settings" },
                new () { Id = 3, Name = "Search History" },
                new () { Id = 4, Name = "Invoice History" },
            };
    }
}