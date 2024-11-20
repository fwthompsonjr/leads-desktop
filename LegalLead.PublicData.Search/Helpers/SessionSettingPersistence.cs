using LegalLead.PublicData.Search.Common;
using System.Collections.Generic;
using System.IO;
using Thompson.RecordSearch.Utility.Extensions;

namespace LegalLead.PublicData.Search.Helpers
{
    public class SessionSettingPersistence : SessionFilePersistence
    {
        public override void Initialize()
        {
            lock (locker)
            {
                var fileName = SetupFile;
                if (string.IsNullOrEmpty(fileName)) return;
                if (File.Exists(fileName) && new FileInfo(fileName).Length > 0) return;
                Write(settings.ToJsonString());
            }
        }
        public List<T> GetList<T>()
        {
            Initialize();
            var content = Read();
            var dto = content.ToInstance<List<T>>();
            return dto ?? new();
        }
        public bool Change(UserSettingChangeViewModel model)
        {
            var dto = GetList<UserSettingChangeModel>();
            if (dto == null || dto.Count == 0) return false;
            var item = dto.Find(x => x.Category == model.Category && x.Name == model.Name);
            if (item == null) return false;
            item.Value = model.Value;
            var json = dto.ToJsonString();
            Write(json);
            return true;
        }
        protected override string SetupFile
        {
            get
            {
                if (setupFileName != null) return setupFileName;
                setupFileName = SessionUtil.GetFullFileName(datFileName);
                return setupFileName;
            }
        }
        private static string setupFileName = null;
        private const string datFileName = "session.usr";
        private static readonly List<UserSettingChangeModel> settings =
            new()
            {
                new(){ Category = "search", Name = "Last County:", Index = 0},
                new(){ Category = "search", Name = "Start Date:", Index = 1},
                new(){ Category = "search", Name = "End Date:", Index = 2},
                new(){ Category = "browser", Name = "Open Headless:", Index = 10, IsSecured = true, Value = "true"},
            };
        private static readonly object locker = new ();
    }
}