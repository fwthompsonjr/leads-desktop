using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Helpers;
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
using System.Threading;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.DriverFactory;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search.Util
{
    public class BexarUiInteractive : BexarWebInteractive
    {
        public BexarUiInteractive(WebNavigationParameter parameters, bool displayDialogue = true) : base(parameters)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            StartDate = FetchKeyedData(parameters.Keys, "StartDate");
            EndingDate = FetchKeyedData(parameters.Keys, "EndDate");
            CourtType = FetchKeyedItem(parameters.Keys, "CourtType");
            DisplayDialogue = displayDialogue;
            var container = ActionBexarContainer.GetContainer;
            var collection = container.GetAllInstances<ICountySearchAction>().ToList();
            collection.Sort((a, b) => a.OrderId.CompareTo(b.OrderId));
            collection.ForEach(c => c.Interactive = this);
            ActionItems.AddRange(collection);
        }
        public List<PersonAddress> People { get; private set; } = new List<PersonAddress>();
        public List<CaseItemDto> Items { get; private set; } = new List<CaseItemDto>();
        protected List<CaseItemDto> CaseStyles { get; private set; } = new List<CaseItemDto>();

        protected bool ExecutionCancelled { get; set; }
        protected bool DisplayDialogue { get; set; }
        protected string CourtType { get; set; }
        private bool IsDateRangeComplete = false;
        private readonly List<ICountySearchAction> ActionItems = new();
        public override WebFetchResult Fetch()
        {
            using var hider = new HideProcessWindowHelper();
            var postsearchtypes = new List<Type> { typeof(BexarFetchFilingDetail) };
            var getitemsearchtypes = new List<Type> { typeof(BexarFetchCaseDetail) };
            var driver = GetDriver(DriverReadHeadless);
            var parameters = new DallasSearchProcess();
            var dates = DallasSearchProcess.GetBusinessDays(StartDate, EndingDate);
            var common = ActionItems.FindAll(a => !postsearchtypes.Contains(a.GetType()));
            var postcommon = ActionItems.FindAll(a => !getitemsearchtypes.Contains(a.GetType()));
            var result = new WebFetchResult();
            Iterate(driver, parameters, dates, common, postcommon);
            if (ExecutionCancelled || People.Count == 0) return result;
            result.PeopleList = People;
            result.Result = GetExcelFileName();
            result.CaseList = JsonConvert.SerializeObject(People);
            return result;
        }

        public virtual IWebDriver GetDriver(bool headless = false)
        {
            return new FireFoxProvider().GetWebDriver(headless);
        }

        public string GenerateExcel()
        {
            try
            {
                return GetExcelFileName();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        protected virtual void Iterate(IWebDriver driver, DallasSearchProcess parameters, List<DateTime> dates, List<ICountySearchAction> common, List<ICountySearchAction> postcommon)
        {
            try
            {
                if (driver == null) throw new ArgumentNullException(nameof(driver));
                if (parameters == null) throw new ArgumentNullException(nameof(parameters));
                if (dates == null) throw new ArgumentNullException(nameof(dates));
                if (common == null) throw new ArgumentNullException(nameof(common));
                if (postcommon == null) throw new ArgumentNullException(nameof(postcommon));

                IterateDateRange(driver, parameters, dates, common);
                IterateItems(driver, parameters, postcommon);
            }
            catch (Exception ex)
            {
                // no action on excpetion thrown.
                Console.WriteLine(ex.Message);
            }
            finally
            {
                driver?.Quit();
            }

        }

        protected virtual void IterateDateRange(IWebDriver driver, DallasSearchProcess parameters, List<DateTime> dates, List<ICountySearchAction> common)
        {
            if (driver == null) throw new ArgumentNullException(nameof(driver));
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            if (dates == null) throw new ArgumentNullException(nameof(dates));
            if (common == null) throw new ArgumentNullException(nameof(common));
            bool isCaptchaNeeded = true;

            dates.ForEach(d =>
            {
                IsDateRangeComplete = false;
                Console.WriteLine("Searching for records on date: {0:d}", d);
                parameters.SetSearchParameters(d, d, CourtType);
                common.ForEach(a =>
                {
                    if (!IsDateRangeComplete)
                    {
                        isCaptchaNeeded = IterateCommonActions(isCaptchaNeeded, driver, parameters, common, a);
                        var unmapped = Items.FindAll(x => string.IsNullOrEmpty(x.CourtDate));
                        unmapped.ForEach(m => { m.CourtDate = d.ToString("d", CultureInfo.CurrentCulture); });
                    }
                });
            });
            IsDateRangeComplete = false;
            parameters.SetSearchParameters(dates[0], dates[^1], CourtType);
        }

        protected virtual void IterateItems(IWebDriver driver, DallasSearchProcess parameters, List<ICountySearchAction> postcommon)
        {
            if (driver == null) throw new ArgumentNullException(nameof(driver));
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            if (postcommon == null) throw new ArgumentNullException(nameof(postcommon));
            if (ExecutionCancelled) return;
            var courtDates = Items.Select(s => s.GetCourtDate())
                .Where(c => c != null)
                .Select(d => d.GetValueOrDefault().Date)
                .Distinct().ToList();
            if (courtDates.Count > 0)
            {
                courtDates.ForEach(d =>
                {
                    Console.WriteLine("Getting case details for: {0:d}", d);
                    parameters.SetSearchParameters(d, d, CourtType);
                    postcommon.ForEach(a =>
                    {
                        IterateCommonActions(false, driver, parameters, postcommon, a);
                    });
                });
            }
            CaseStyles.ForEach(c =>
            {
                var targets = Items.FindAll(x => x.CaseNumber == c.CaseNumber);
                targets.ForEach(t => AssignFilingDateAndPartyName(c, t));
            });
            var casenumbers = Items.Select(s => s.CaseNumber).Distinct().ToList();
            casenumbers.ForEach(i =>
            {
                var p = People.Find(x => x.CaseNumber == i);
                if (p == null)
                {
                    var source = Items.Find(x => x.CaseNumber == i);
                    if (source != null) { AppendPerson(source); }
                }
            });
        }

        private bool IterateCommonActions(bool isCaptchaNeeded, IWebDriver driver, DallasSearchProcess parameters, List<ICountySearchAction> common, ICountySearchAction a)
        {
            if (ExecutionCancelled) return isCaptchaNeeded;
            var count = common.Count - 1;
            Populate(a, driver, parameters);
            var response = a.Execute();
            if (a is BexarAuthenicateActor && response is string loginPage)
            {
                common.FindAll(c => c.GetType() == typeof(BexarAuthenicateBegin))
                    .ForEach(ab =>
                    {
                        if (ab is BexarAuthenicateBegin bb &&
                        string.IsNullOrEmpty(bb.LoginAddress)) bb.LoginAddress = loginPage;
                    });
            }
            if (a is BexarFetchCaseDetail _ && response is string cases) Items.AddRange(GetData(cases));
            if (a is BexarFetchFilingDetail _ && response is string details) CaseStyles.AddRange(GetData(details));
            if (a is BexarSetNoCountVerification _ && response is bool hasNoCount && hasNoCount)
            {
                IsDateRangeComplete = true;
            }
            if (common.IndexOf(a) != count) Thread.Sleep(750); // add pause to be more human in interaction
            return isCaptchaNeeded;
        }

        private static void AssignFilingDateAndPartyName(CaseItemDto c, CaseItemDto target)
        {
            target.FileDate = c.FileDate;
            if (string.IsNullOrWhiteSpace(target.PartyName) && !string.IsNullOrWhiteSpace(c.PartyName))
                target.PartyName = c.PartyName;
            if (!string.IsNullOrWhiteSpace(c.Court)) target.Court = c.Court;
            if (!string.IsNullOrWhiteSpace(c.Address)) target.Address = c.Address;
        }

        private static void Populate(ICountySearchAction a, IWebDriver driver, DallasSearchProcess parameters)
        {
            a.Driver = driver;
            a.Parameters = parameters;
        }

        private void AppendPerson(CaseItemDto dto)
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

        [ExcludeFromCodeCoverage]
        private static List<CaseItemDto> GetData(string json)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<List<CaseItemDto>>(json);
                if (data == null) return new List<CaseItemDto>();
                return data;
            }
            catch (Exception)
            {
                return new List<CaseItemDto>();
            }
        }

        private string GetExcelFileName()
        {
            var folder = ExcelDirectoyName();
            var name = DallasSearchProcess.GetCourtName(CourtType);
            var fmt = $"BEXAR_{name}_{GetDateString(StartDate)}_{GetDateString(EndingDate)}";
            var fullName = Path.Combine(folder, $"{fmt}.xlsx");
            var idx = 1;
            while (File.Exists(fullName))
            {
                fullName = Path.Combine(folder, $"{fmt}_{idx:D4}.xlsx");
                idx++;
            }
            var writer = new ExcelWriter();
            var content = writer.ConvertToPersonTable(addressList: People, worksheetName: "addresses", websiteId: 80);
            var courtlist = People.Select(p =>
            {
                if (string.IsNullOrEmpty(p.Court)) return string.Empty;
                var find = BexarCourtLookupService.GetAddress(name, p.Court);
                if (string.IsNullOrEmpty(find)) return string.Empty;
                return find;
            }).ToList();
            content.TransferColumn("County", "fname");
            content.TransferColumn("CourtAddress", "lname");
            content.PopulateColumn("CourtAddress", courtlist);
            using (var ms = new MemoryStream())
            {
                content.SaveAs(ms);
                var data = ms.ToArray();
                File.WriteAllBytes(fullName, data);
            }
            return fullName;
        }

        private static string GetDateString(DateTime date)
        {
            const string fmt = "yyMMdd";
            return date.ToString(fmt, Culture);
        }

        private static string ExcelDirectoyName()
        {
            var appFolder =
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var xmlFolder = Path.Combine(appFolder, "data");
            if (!Directory.Exists(xmlFolder)) Directory.CreateDirectory(xmlFolder);
            return xmlFolder;
        }
        private static readonly CultureInfo Culture = CultureInfo.CurrentCulture;
    }
}