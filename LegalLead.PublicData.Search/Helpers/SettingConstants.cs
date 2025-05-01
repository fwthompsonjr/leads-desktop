namespace LegalLead.PublicData.Search.Helpers
{
    internal static class SettingConstants
    {
        internal static class Categories
        {
            public const string Admin = "admin";
            public const string Search = "search";
        }
        internal static class DataTypeName
        {
            public const string Boolean = "Bool";
            public const string DateTime = "DateTime";
            public const string Numeric = "Numeric";
        }
        internal static class AdminFieldNames
        {
            public const string AllowBrowserDisplay = "Open Headless:";
            public const string AllowDbSearching = "Database Search:";
            public const string ExtendedDateMaxDays = "Extended Date Range:";
            public const string DbMinimumPersistenceDays = "Database Minimum Persistence:";
        }
        internal static class SearchFieldNames
        {
            public const string AllowOfflineProcessing = "Allow Offline Data Processing:";
            public const string LastCounty = "Last County:";
            public const string StartDate = "Start Date:";
            public const string EndDate = "End Date:";
            public const string ExcludeWeekends = "Exclude Weekend From Search:";
        }
    }
}
