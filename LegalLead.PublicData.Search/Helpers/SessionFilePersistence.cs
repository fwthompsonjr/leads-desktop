using LegalLead.PublicData.Search.Interfaces;
using System.IO;

namespace LegalLead.PublicData.Search.Helpers
{
    internal class SessionFilePersistence : ISessionPersistance
    {
        public virtual string GetAccountCredential(string county = "")
        {
            var data = Read();
            if (string.IsNullOrWhiteSpace(data)) return string.Empty;
            var dto = SessionUtil.GetPermissionsMap(data);
            return dto?.CountyPermission ?? string.Empty;
        }

        public void Initialize()
        {
            lock (locker)
            {
                var fileName = SetupFile;
                if (string.IsNullOrEmpty(fileName)) return;
                if (File.Exists(fileName)) File.Delete(fileName);
                File.WriteAllText(fileName, "");
            }
        }

        public string Read()
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

        public bool Write(string content)
        {
            lock (locker)
            {
                var fileName = SetupFile;
                if (string.IsNullOrEmpty(fileName)) return false;
                if (string.IsNullOrWhiteSpace(content)) return false;
                try
                {
                    var data = content.Crypt();
                    if (File.Exists(fileName)) File.Delete(fileName);
                    File.WriteAllText(fileName, data);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        protected virtual string SetupFile
        {
            get
            {
                if (setupFileName != null) return setupFileName;
                setupFileName = SessionUtil.GetFullFileName(datFileName);
                return setupFileName;
            }
        }
        private static string setupFileName = null;
        private const string datFileName = "session.dat";
        private static readonly object locker = new();
    }
}
