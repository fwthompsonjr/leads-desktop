using LegalLead.PublicData.Search.Common;
using LegalLead.PublicData.Search.Extensions;
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
        public T GetSettingOrDefault<T>(string category, string key, T defaultValue)
        {
            var dto = GetList<UserSettingChangeModel>();
            if (dto == null || dto.Count == 0) return defaultValue;
            var item = dto.Find(x => x.Category == category && x.Name == key);
            if (item == null) return defaultValue;
            return item.Value.ChangeType(defaultValue);
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
                new(){
                    Category = "search",
                    Name = "Last County:",
                    DataType = "DateTime",
                    Index = 0},
                new(){
                    Category = "search",
                    Name = "Start Date:",
                    DataType = "DateTime",
                    Index = 1},
                new(){
                    Category = "search",
                    Name = "End Date:",
                    Index = 2},

                new(){
                    Category = "search",
                    Name = "Exclude Weekend From Search:",
                    DataType = "Bool",
                    Value = "true",
                    Index = 3},
                new(){
                    Category = "admin",
                    Name = "Open Headless:",
                    Index = 10,
                    IsSecured = true,
                    DataType = "Bool",
                    Value = "true"},
                new(){
                    Category = "admin",
                    Name = "Database Search:",
                    Index = 12,
                    IsSecured = true,
                    DataType = "Bool",
                    Value = "true"},
                new(){
                    Category = "admin",
                    Name = "Database Minimun Persistence:",
                    Index = 14,
                    IsSecured = true,
                    DataType = "Numeric",
                    Value = "5"},
                new(){
                    Category = "admin",
                    Name = "Extended Date Range:",
                    Index = 16,
                    IsSecured = true,
                    DataType = "Numeric",
                    Value = "7"},
            };
        private static readonly object locker = new();
    }
}