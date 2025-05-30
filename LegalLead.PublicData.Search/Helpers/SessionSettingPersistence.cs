﻿using LegalLead.PublicData.Search.Common;
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
                    Category = SettingConstants.Categories.Search,
                    Name = SettingConstants.SearchFieldNames.LastCounty,
                    Index = 0},
                new(){
                    Category = SettingConstants.Categories.Search,
                    Name = SettingConstants.SearchFieldNames.StartDate,
                    DataType = SettingConstants.DataTypeName.DateTime,
                    Index = 1},
                new(){
                    Category = SettingConstants.Categories.Search,
                    Name = SettingConstants.SearchFieldNames.EndDate,
                    DataType = SettingConstants.DataTypeName.DateTime,
                    Index = 2},

                new(){
                    Category = SettingConstants.Categories.Search,
                    Name = SettingConstants.SearchFieldNames.ExcludeWeekends,
                    DataType = SettingConstants.DataTypeName.Boolean,
                    Value = "true",
                    Index = 3},
                new(){
                    Category = SettingConstants.Categories.Search,
                    Name = SettingConstants.SearchFieldNames.AllowOfflineProcessing,
                    DataType = SettingConstants.DataTypeName.Boolean,
                    Value = "true",
                    Index = 4},
                new(){
                    Category = SettingConstants.Categories.Admin,
                    Name = SettingConstants.AdminFieldNames.AllowBrowserDisplay,
                    Index = 10,
                    IsSecured = true,
                    DataType = SettingConstants.DataTypeName.Boolean,
                    Value = "true"},
                new(){
                    Category = SettingConstants.Categories.Admin,
                    Name = SettingConstants.AdminFieldNames.AllowDbSearching,
                    Index = 12,
                    IsSecured = true,
                    DataType = SettingConstants.DataTypeName.Boolean,
                    Value = "true"},
                new(){
                    Category = SettingConstants.Categories.Admin,
                    Name = SettingConstants.AdminFieldNames.DbMinimumPersistenceDays,
                    Index = 14,
                    IsSecured = true,
                    DataType = SettingConstants.DataTypeName.Numeric,
                    Value = "5"},
                new(){
                    Category = SettingConstants.Categories.Admin,
                    Name = SettingConstants.AdminFieldNames.ExtendedDateMaxDays,
                    Index = 16,
                    IsSecured = true,
                    DataType = SettingConstants.DataTypeName.Numeric,
                    Value = "7"},
            };
        private static readonly object locker = new();
    }
}