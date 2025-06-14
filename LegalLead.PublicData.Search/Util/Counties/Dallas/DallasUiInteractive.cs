using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Extensions;
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
using System.Windows.Forms;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.DriverFactory;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class DallasUiInteractive : DallasWebInteractive
    {
        public DallasUiInteractive(WebNavigationParameter parameters, bool displayDialogue = true) : base(parameters)
        {
            ArgumentNullException.ThrowIfNull(parameters);
            StartDate = FetchKeyedData(parameters.Keys, "StartDate");
            EndingDate = FetchKeyedData(parameters.Keys, "EndDate");
            CourtType = FetchKeyedItem(parameters.Keys, "CourtType");
            DisplayDialogue = displayDialogue;
            var container = ActionDallasContainer.GetContainer;
            var collection = container.GetAllInstances<ICountySearchAction>().ToList();
            collection.Sort((a, b) => a.OrderId.CompareTo(b.OrderId));
            ActionItems.AddRange(collection);
        }

        public List<PersonAddress> People { get; private set; } = [];
        public List<CaseItemDto> Items { get; private set; } = [];
        public bool IsDistrictFilterActive { get; set; }
        protected bool OfflineProcessingEnabled { get; set; }
        protected List<DallasCaseStyleDto> CaseStyles { get; private set; } = [];
        protected bool ExecutionCancelled { get; set; }
        protected bool DisplayDialogue { get; set; }
        internal string CourtType { get; set; }
        private readonly List<ICountySearchAction> ActionItems = [];
        public override WebFetchResult Fetch(CancellationToken token)
        {
            try
            {

                using var hider = new HideProcessWindowHelper();
                var postsearchtypes = new List<Type> { typeof(DallasFetchCaseStyle) };
                var driver = GetDriver(false);
                var parameters = new DallasSearchProcess();
                var dates = DallasSearchProcess.GetBusinessDays(StartDate, EndingDate, true, true);
                var common = ActionItems.FindAll(a => !postsearchtypes.Contains(a.GetType()));
                var postcommon = ActionItems.FindAll(a => postsearchtypes.Contains(a.GetType()));
                OfflineProcessingEnabled = SettingsWriter.GetSettingOrDefault("search",
                    SettingConstants.SearchFieldNames.AllowOfflineProcessing,
                    true);
                var bulkReader = new DallasBulkCaseReader
                {
                    Driver = driver,
                    Interactive = this,
                    Parameters = parameters,
                    OfflineProcessing = OfflineProcessingEnabled,
                };
                postcommon.Add(bulkReader);


                var result = new WebFetchResult();
                var excludeWeekend = SettingsWriter.GetSettingOrDefault("search", SettingConstants.SearchFieldNames.ExcludeWeekends, true);

                var weekends = new[] { DayOfWeek.Saturday, DayOfWeek.Sunday };
                if (excludeWeekend)
                {
                    dates.RemoveAll(d => weekends.Contains(d.DayOfWeek));
                }
                Iterate(driver, parameters, dates, common, postcommon);
                if (OfflineProcessingEnabled || ExecutionCancelled || People.Count == 0) return result;
                People.RemoveAll(x => string.IsNullOrWhiteSpace(x.DateFiled));
                result.PeopleList = People;
                result.Result = GetExcelFileName();
                result.CaseList = JsonConvert.SerializeObject(People);
                return result;
            }
            finally
            {
                ReportProgessComplete?.Invoke();
            }
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
                ArgumentNullException.ThrowIfNull(driver);
                ArgumentNullException.ThrowIfNull(parameters);
                ArgumentNullException.ThrowIfNull(dates);
                ArgumentNullException.ThrowIfNull(common);
                ArgumentNullException.ThrowIfNull(postcommon);

                IterateDateRange(driver, parameters, dates, common);
                postcommon.ForEach(p =>
                {
                    if (p is DallasBulkCaseReader bulk)
                    {
                        bulk.Workload.Clear();
                        bulk.Workload.AddRange(Items);
                    }
                });
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
            ArgumentNullException.ThrowIfNull(driver);
            ArgumentNullException.ThrowIfNull(parameters);
            ArgumentNullException.ThrowIfNull(dates);
            ArgumentNullException.ThrowIfNull(common);
            bool isCaptchaNeeded = true;
            var exec = (IJavaScriptExecutor)driver;
            var iterator = IterationProvider.GetIterator(
                CourtType,
                driver,
                exec);
            dates.ForEach(d =>
            {
                Console.WriteLine("Searching for records on date: {0:d}", d);
                parameters.SetSearchParameters(d, d, CourtType);
                iterator.SearchIndex = 0;
                var collection = iterator.GetCollection().FindAll(x => !x.IsExecuted);
                while (collection.Any(a => !a.IsExecuted))
                {
                    isCaptchaNeeded = IterateCourtLocations(driver, parameters, common, d, isCaptchaNeeded, iterator, collection);
                    collection = collection.FindAll(x => !x.IsExecuted);
                }

                this.EchoProgess(0, 0, 0, dateNotification: "hide");
            });
            this.CompleteProgess();
            parameters.SetSearchParameters(dates[0], dates[^1], CourtType);
        }

        private bool IterateCourtLocations(IWebDriver driver, DallasSearchProcess parameters, List<ICountySearchAction> common, DateTime d, bool isCaptchaNeeded, ICaseTypeIterator iterator, List<CaseTypeExecutionTracker> collection)
        {
            try
            {
                bool isDistrict = collection.Any(x => x.Officer.Court.StartsWith("DC"));
                if (isDistrict && !IsDistrictFilterActive)
                {
                    collection.ForEach(c => c.IsExecuted = true);
                    collection.RemoveAll(x => x.Id > 0);
                }
                var limit = collection.Count;
                foreach (var item in collection)
                {
                    iterator.SearchIndex = collection.IndexOf(item) + 1;
                    this.EchoProgess(0, limit, iterator.SearchIndex, "", false, "", $"{d:M/d}-{iterator.SearchIndex}");
                    common.ForEach(a =>
                    {
                        if (a is DallasNavigateSearch navSearch)
                        {
                            navSearch.CaseTypeIterator = iterator;
                        }
                        isCaptchaNeeded = IterateCommonActions(isCaptchaNeeded, driver, parameters, common, a);
                        if (a is DallasSetupParameters)
                        {
                            iterator.SetParameter(collection);
                        }
                    });
                }
            }
            catch (Exception)
            {
                // take no action on error here
            }

            return isCaptchaNeeded;
        }

        private bool IterateCommonActions(bool isCaptchaNeeded, IWebDriver driver, DallasSearchProcess parameters, List<ICountySearchAction> common, ICountySearchAction a)
        {
            if (ExecutionCancelled) return isCaptchaNeeded;
            if (!isCaptchaNeeded && a is DallasAuthenicateBegin _) return isCaptchaNeeded;
            if (!isCaptchaNeeded && a is DallasAuthenicateSubmit _) return isCaptchaNeeded;
            if (!isCaptchaNeeded && a is DallasRequestCaptcha _) return isCaptchaNeeded;
            var count = common.Count - 1;
            Populate(a, driver, parameters);
            if (a is BaseDallasSearchAction preaction) { preaction.MaskUserName(); }
            object response = ExecuteAction(a);
            if (a is BaseDallasSearchAction postaction) { postaction.MaskUserName(); }
            if (a is DallasAuthenicateSubmit _ && response is bool captchaCompleted && captchaCompleted) isCaptchaNeeded = false;
            if (a is DallasFetchCaseDetail _ && response is string cases) Items.AddRange(GetData(cases));
            if (a is DallasRequestCaptcha _ && response is bool canExecute && !canExecute) ExecutionCancelled = true;
            if (common.IndexOf(a) != count) Thread.Sleep(750); // add pause to be more human in interaction
            return isCaptchaNeeded;
        }

        private static object ExecuteAction(ICountySearchAction a)
        {
            try
            {
                if (a is DallasNavigateSearch navigateSearch)
                {
                    return SearchActionWrapper.Execute(navigateSearch);
                }
                return a.Execute();
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine(e.ToString());
                return null;
#else
                return null;
#endif
            }
        }

        protected virtual void IterateItems(IWebDriver driver, DallasSearchProcess parameters, List<ICountySearchAction> postcommon)
        {
            ArgumentNullException.ThrowIfNull(driver);
            ArgumentNullException.ThrowIfNull(parameters);
            ArgumentNullException.ThrowIfNull(postcommon);
            if (ExecutionCancelled) return;
            Items.Sort((a, b) => a.FileDate.CompareTo(b.FileDate));
            postcommon.ForEach(a =>
            {
                var resp = ExecuteAction(a);
                if (resp is string json)
                {
                    var list = json.ToInstance<List<CaseItemDto>>();
                    if (list != null) Items = list;
                }
            });

            var casenumbers = Items.Select(s => s.CaseNumber).Distinct().ToList();
            CaseStyles = [];
            casenumbers.ForEach(i =>
            {
                var sources = Items.FindAll(x => x.CaseNumber.Equals(i));
                var source = sources[0];
                if (sources.Count > 1 || sources.Any(x => !string.IsNullOrEmpty(x.Address)))
                {
                    source = sources.Find(x => !string.IsNullOrEmpty(x.Address));
                }
                CaseStyles.Add(new() { Address = source.Address, CaseStyle = source.CaseStyle, Plaintiff = source.Plaintiff });
                var p = People.Find(x => x.CaseNumber == i);
                if (p == null)
                {
                    var itmDto = Items.Find(x => x.CaseNumber == i);
                    if (itmDto != null) { AppendPerson(itmDto); }
                }
            });
            this.EchoProgess(0, 0, 0, dateNotification: "hide");
            this.CompleteProgess();
        }

        private void Populate(ICountySearchAction a, IWebDriver driver, DallasSearchProcess parameters, string uri = "")
        {
            a.Driver = driver;
            a.Parameters = parameters;
            if (a is DallasRequestCaptcha captcha) { captcha.PromptUser = UserPrompt; }
            if (!string.IsNullOrEmpty(uri) && a is DallasFetchCaseStyle style) { style.PageAddress = uri; }
            if (a is DallasFetchCaseItems items) { items.PauseForPage = true; }
        }

        [ExcludeFromCodeCoverage]
        private bool UserPrompt()
        {
            if (!DisplayDialogue)
            {
                Thread.Sleep(100);
                return true;
            }
            return DisplayUserCaptchaHelper.UserPrompt();
        }

        private void AppendPerson(CaseItemDto dto)
        {
            var person = dto.FromDto();
            var target = CaseStyles.Find(c => c.CaseStyle.Equals(dto.CaseStyle, StringComparison.OrdinalIgnoreCase));
            if (target == null && !string.IsNullOrEmpty(dto.CaseStyle))
            {
                CaseStyles.Add(new()
                {
                    Address = dto.Address,
                    CaseStyle = dto.CaseStyle,
                    Plaintiff = dto.Plaintiff
                });
                AppendPerson(dto);
                return;
            }
            var address = GetAddress(target);

            if (address != null && address.Count != 0)
            {
                var ln = address.Count - 1;
                var last = address[ln].Trim();
                var pieces = last.Split(' ');
                person.Zip = pieces[^1];
                person.Address3 = last;
                person.Address1 = address[0];
                person.Address2 = string.Empty;
                if (ln > 1)
                {
                    address.RemoveAt(0); // remove first item
                    if (address.Count > 1) address.RemoveAt(address.Count - 1); // remove last, when applicable
                    person.Address2 = string.Join(" ", address);
                }
            }
            People.Add(person);
        }

        private static List<string> GetAddress(DallasCaseStyleDto dto)
        {
            var pipe = "|";
            var doublepipe = "||";
            if (dto == null || string.IsNullOrEmpty(dto.Address)) return null;
            var data = dto.Address;
            while (data.Contains(doublepipe)) { data = data.Replace(doublepipe, pipe); }
            return [.. data.Split('|')];
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
                if (data == null) return [];
                return data;
            }
            catch (Exception)
            {
                return [];
            }
        }

        private string GetExcelFileName()
        {
            var folder = ExcelDirectoyName();
            var name = DallasSearchProcess.GetCourtName(CourtType);
            var fmt = $"DALLAS_{name}_{GetDateString(StartDate)}_{GetDateString(EndingDate)}";
            var fullName = Path.Combine(folder, $"{fmt}.xlsx");
            var idx = 1;
            while (File.Exists(fullName))
            {
                fullName = Path.Combine(folder, $"{fmt}_{idx:D4}.xlsx");
                idx++;
            }
            var writer = new ExcelWriter();
            var content = writer.ConvertToPersonTable(addressList: People, worksheetName: "addresses", websiteId: 60);
            var courtlist = People.Select(p =>
            {
                if (string.IsNullOrEmpty(p.Court)) return string.Empty;
                var find = DallasCourtLookupService.GetAddress(name, p.Court);
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

        private static class IterationProvider
        {
            public static ICaseTypeIterator GetIterator(string name,
            IWebDriver web,
            IJavaScriptExecutor executor)
            {
                const StringComparison oic = StringComparison.OrdinalIgnoreCase;
                if (name.Equals("JUSTICE", oic)) return new DallasJusticeHelper(web, executor);
                if (name.Equals("COUNTY", oic)) return new DallasCountyHelper(web, executor);
                if (name.Equals("DISTRICT", oic)) return new DallasDistrictHelper(web, executor);
                return new FallbackIterator();
            }
        }
        private static readonly CultureInfo Culture = CultureInfo.CurrentCulture;

        private class FallbackIterator : BaseCaseIterator
        {
            public override string Name => "FALLBACK";

            public override List<Common.DallasJusticeOfficer> Officers => throw new NotImplementedException();

            public override string JsContentScript { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
        }

        private class SetParameterResponse
        {
            // conflicting lint from (sonarqube, ide) messages are forcing duplicate suppression methods
#pragma warning disable IDE0079 // Remove unnecessary suppression
            [SuppressMessage("Minor Code Smell", "S3459:Unassigned members should be removed", Justification = "Setter is needed for data-binding")]

            public int Id { get; set; }
            [SuppressMessage("Minor Code Smell", "S3459:Unassigned members should be removed", Justification = "Setter is needed for data-binding")]
            public bool Result { get; set; }
#pragma warning restore IDE0079 // Remove unnecessary suppression
        }
        private static readonly SessionSettingPersistence SettingsWriter =
            SessionPersistenceContainer
            .GetContainer
            .GetInstance<SessionSettingPersistence>();
    }
}
