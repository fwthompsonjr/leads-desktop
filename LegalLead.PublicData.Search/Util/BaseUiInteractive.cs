using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Extensions;
using LegalLead.PublicData.Search.Interfaces;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.DriverFactory;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search.Util
{
    public abstract class BaseUiInteractive : BaseWebIneractive
    {
        public List<PersonAddress> People { get; private set; } = [];
        public List<CaseItemDto> Items { get; private set; } = [];
        protected List<CaseItemDto> CaseStyles { get; private set; } = [];
        protected readonly List<ICountySearchAction> ActionItems = [];

        protected BaseUiInteractive(WebNavigationParameter parameters)
        {
            ArgumentNullException.ThrowIfNull(parameters);
            StartDate = FetchKeyedData(parameters.Keys, "StartDate");
            EndingDate = FetchKeyedData(parameters.Keys, "EndDate");
            CourtType = FetchKeyedItem(parameters.Keys, "CourtType");
            AssignOptionalKeyValues(parameters);
        }

        [ExcludeFromCodeCoverage(Justification = "Interacts with system, creating web browser component")]
        public virtual IWebDriver GetDriver(bool headless = false)
        {
            return new FireFoxProvider().GetWebDriver(DriverReadHeadless);
        }

        protected abstract string GetCourtAddress(string courtType, string court);

        protected void AppendPerson(CaseItemDto dto)
        {
            var person = dto.FromDto();
            if (!string.IsNullOrWhiteSpace(dto.Address))
            {
                var address = dto.Address;
                var parts = address.Split('|').ToList();
                person.UpdateAddress(parts);
            }
            People.Add(person);
        }

        protected string GenerateExcelFile(string countyName, int websiteId, bool isTest = false)
        {
            var folder = GetExcelDirectoryName;
            var name = DallasSearchProcess.GetCourtName(CourtType);
            var fmt = $"{countyName}_{name}_{GetDateString(StartDate)}_{GetDateString(EndingDate)}";
            fmt = GetContextFileName(websiteId, countyName, fmt);
            var fullName = GetUniqueFileName(folder, fmt, Path.Combine(folder, $"{fmt}.xlsx"));
            var writer = new ExcelWriter();
            var content = writer.ConvertToPersonTable(addressList: People, worksheetName: "addresses", websiteId: websiteId);
            var courtlist = People.Select(p =>
            {
                if (string.IsNullOrEmpty(p.Court)) return string.Empty;
                var find = GetCourtAddress(name, p.Court);
                if (string.IsNullOrEmpty(find)) return string.Empty;
                return find;
            }).ToList();
            content.TransferColumn("County", "fname");
            content.TransferColumn("CourtAddress", "lname");
            content.PopulateColumn("CourtAddress", courtlist);
            content.SecureContent(TrackingIndex);
            using (var ms = new MemoryStream())
            {
                content.SaveAs(ms);
                var data = ms.ToArray();
                if (!isTest) File.WriteAllBytes(fullName, data);
            }
            return fullName;
        }
        private string GetContextFileName(int websiteId, string countyName, string calculatedName)
        {
            if (websiteId != 10) return calculatedName;
            var context = UserSelectedSearchName.ToUpperInvariant();
            var filter = "JUSTICE";
            if (UserSelectedCourtType.Contains("Probate")) filter = "PROBATE";
            if (UserSelectedCourtType.Contains("CCL")) filter = "COUNTY";
            if (UserSelectedCourtType.Contains("JP") && !UserSelectedCourtType.Contains("All"))
            {
                var indx = UserSelectedCourtType.Split(' ')[^1];
                filter = string.Concat(filter, "_", indx);
            }
            var newName = $"{countyName}_{context}_{filter}_{GetDateString(StartDate)}_{GetDateString(EndingDate)}";
            return newName;
        }

        protected string CourtType { get; set; }
        protected string UserSelectedCourtType { get; set; } = "All JP Courts";
        protected string UserSelectedSearchName { get; set; } = "Civil";
        protected bool IsDateRangeComplete { get; set; }

        protected static string GetExcelDirectoryName => excelDirectoyName ??= ExcelDirectoyName();

        protected static void Populate(ICountySearchAction a, IWebDriver driver, DallasSearchProcess parameters)
        {
            a.Driver = driver;
            a.Parameters = parameters;
        }

        [ExcludeFromCodeCoverage]
        protected static List<CaseItemDto> GetData(string json)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<List<CaseItemDto>>(json);
                if (data == null) return [];
                return data;
            }
            catch (Exception)
            {
                return [];
            }
        }

        private void AssignOptionalKeyValues(WebNavigationParameter parameters)
        {
            var userSelectedItem = FetchKeyedItem(parameters.Keys, "UserSelectedCourtType");
            var userSelectedContext = FetchKeyedItem(parameters.Keys, "UserSelectedSearchName");
            if (!string.IsNullOrEmpty(userSelectedItem))
            {
                UserSelectedCourtType = userSelectedItem;
            }
            if (!string.IsNullOrEmpty(userSelectedContext))
            {
                UserSelectedSearchName = userSelectedContext;
            }
        }

        private static string FetchKeyedItem(List<WebNavigationKey> keys, string keyname)
        {
            var item = (keys.Find(x => x.Name == keyname)?.Value) ?? throw new ArgumentOutOfRangeException(nameof(keyname));
            return item;
        }

        private static DateTime FetchKeyedData(List<WebNavigationKey> keys, string keyname)
        {
            var item = FetchKeyedItem(keys, keyname);
            if (!DateTime.TryParse(item, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out var dte))
                throw new ArgumentOutOfRangeException(nameof(keyname));
            return dte;
        }


        private static string ExcelDirectoyName()
        {
            var appFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var xmlFolder = Path.Combine(appFolder, "data");
            if (!Directory.Exists(xmlFolder)) Directory.CreateDirectory(xmlFolder);
            return xmlFolder;
        }

        private static string GetDateString(DateTime date)
        {
            const string fmt = "yyMMdd";
            return date.ToString(fmt, culture);
        }

        private static string GetUniqueFileName(string folder, string fmt, string fullName)
        {
            int idx = 1;
            while (File.Exists(fullName))
            {
                fullName = Path.Combine(folder, $"{fmt}_{idx:D4}.xlsx");
                idx++;
            }
            return fullName;
        }

        private static string excelDirectoyName = null;
        private static readonly CultureInfo culture = CultureInfo.CurrentCulture;
    }
}
