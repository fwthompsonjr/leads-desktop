using LegalLead.PublicData.Search.Interfaces;
using System.IO;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Interfaces;

namespace LegalLead.PublicData.Search.Helpers
{
    public class SessionFilePersistence : ISessionPersistance
    {
        public SessionFilePersistence()
        {
            Reader = new CountyCodeReaderService(new HttpService(), new CountyCodeService());
        }
        protected ICountyCodeReader Reader
        { get; set; }
        public virtual string GetAccountPermissions()
        {
            lock (locker)
            {
                var dto = Read().ToInstance<PermissionMapDto>();
                if (dto == null || string.IsNullOrEmpty(dto.WebPermissions)) return string.Empty;
                return dto.WebPermissions;
            }
        }

        public virtual string GetAccountCredential(string county = "")
        {
            lock (locker)
            {
                if (countyAccessCode != null) return countyAccessCode;
                var dallasId = (int)SourceType.DallasCounty;
                var data = Read();
                if (string.IsNullOrWhiteSpace(data)) return string.Empty;
                var dto = SessionUtil.GetPermissionsMap(data);
                var userId = dto?.CountyPermission;
                if (string.IsNullOrEmpty(userId)) return string.Empty;
                var response = Reader.GetCountyCode(dallasId, userId);
                if (string.IsNullOrEmpty(response)) return userId;
                countyAccessCode = response;
                return countyAccessCode;
            }
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
        private static string countyAccessCode = null;
        private static string setupFileName = null;
        private const string datFileName = "session.dat";
        private static readonly object locker = new();
    }
}
