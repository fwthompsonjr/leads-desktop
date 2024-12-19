using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Extensions;
using LegalLead.PublicData.Search.Interfaces;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class TravisUiInteractive : TravisWebInteractive
    {
        public TravisUiInteractive(WebNavigationParameter parameters, bool displayDialogue = true) : base(parameters)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            StartDate = FetchKeyedData(parameters.Keys, "StartDate");
            EndingDate = FetchKeyedData(parameters.Keys, "EndDate");
            CourtType = FetchKeyedItem(parameters.Keys, "CourtType");
            DisplayDialogue = displayDialogue;
            var isJustice = CourtType.Equals("JUSTICE", StringComparison.OrdinalIgnoreCase);
            var container = isJustice ?
                ActionTravisContainer.GetContainer :
                ActionTravisContainer.GetNonJusticeContainer;
            var collection = container.GetAllInstances<ITravisSearchAction>().ToList();
            collection.Sort((a, b) => a.OrderId.CompareTo(b.OrderId));
            ActionItems.AddRange(collection);
            ActionItems.ForEach(item => { item.Interactive = this; });
        }
        public List<PersonAddress> People { get; private set; } = new List<PersonAddress>();
        public List<CaseItemDto> Items { get; private set; } = new List<CaseItemDto>();
        protected List<TravisCaseStyleDto> CaseStyles { get; private set; } = new List<TravisCaseStyleDto>();

        protected bool ExecutionCancelled { get; set; }
        protected bool DisplayDialogue { get; set; }
        protected string CourtType { get; set; }
        private readonly List<ITravisSearchAction> ActionItems = new();
        public override WebFetchResult Fetch()
        {
            try
            {
                var postsearchtypes = new List<Type> { };
                var parameters = new TravisSearchProcess();
                var dates = TravisSearchProcess.GetBusinessDays(StartDate, EndingDate);
                var common = ActionItems.FindAll(a => !postsearchtypes.Contains(a.GetType()));
                var postcommon = ActionItems.FindAll(a => postsearchtypes.Contains(a.GetType()));
                var driver = GetDriver(DriverReadHeadless);
                var result = new WebFetchResult();
                Iterate(driver, parameters, dates, common, postcommon);
                if (ExecutionCancelled || People.Count == 0) return result;
                if (People.Exists(x => string.IsNullOrWhiteSpace(x.Name) && !string.IsNullOrEmpty(x.CaseStyle)))
                {
                    People.ReMapNameFromCaseStyle();
                }
                result.PeopleList = People;
                result.Result = GetExcelFileName();
                result.CaseList = JsonConvert.SerializeObject(People);
                return result;
            }
            finally
            {
                this.CompleteProgess();
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
        protected virtual void Iterate(IWebDriver driver, TravisSearchProcess parameters, List<DateTime> dates, List<ITravisSearchAction> common, List<ITravisSearchAction> postcommon)
        {
            try
            {
                if (driver == null) throw new ArgumentNullException(nameof(driver));
                if (parameters == null) throw new ArgumentNullException(nameof(parameters));
                if (dates == null) throw new ArgumentNullException(nameof(dates));
                if (common == null) throw new ArgumentNullException(nameof(common));
                if (postcommon == null) throw new ArgumentNullException(nameof(postcommon));
                var dcount = dates.Count - 1;
                var dtStart = dates[0];
                var dtEnding = dates[dcount];
                parameters.Search(dtStart, dtEnding, CourtType);
                parameters.CourtLocator.ForEach(location =>
                {
                    var locationId = parameters.CourtLocator.IndexOf(location);
                    if (locationId != 0)
                    {
                        common[0].Execute();
                    }
                    IterateDateRange(driver, parameters, dates, common, locationId);
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

        protected virtual void IterateDateRange(IWebDriver driver, TravisSearchProcess parameters, List<DateTime> dates, List<ITravisSearchAction> common, int locationId)
        {
            if (driver == null) throw new ArgumentNullException(nameof(driver));
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            if (dates == null) throw new ArgumentNullException(nameof(dates));
            if (common == null) throw new ArgumentNullException(nameof(common));
            bool isCaptchaNeeded = true;
            dates.ForEach(d =>
            {
                Console.WriteLine($"Date: {d:d}. Searching {CourtType}");
                parameters.Search(d, d, CourtType);
                common.ForEach(a =>
                {
                    isCaptchaNeeded = IterateCommonActions(isCaptchaNeeded, driver, parameters, a, locationId);
                });
            });
            parameters.Search(dates[0], dates[^1], CourtType);
        }

        private bool IterateCommonActions(bool isCaptchaNeeded, IWebDriver driver, TravisSearchProcess parameters, ITravisSearchAction a, int locationId)
        {
            if (ExecutionCancelled) return isCaptchaNeeded;
            if (!isCaptchaNeeded && a is TravisRequestCaptcha _) return isCaptchaNeeded;

            Populate(a, driver, parameters, locationId);
            var response = a.Execute();
            if (a is TravisFetchCaseItems _ && response is string cases) Items.AddRange(GetData(cases));
            if (a is TravisFetchClickStyle _ && response is string mapping) CaseStyles.AddRange(GetMapping(mapping));
            if (a is TravisRequestCaptcha _ && response is bool canExecute && !canExecute) ExecutionCancelled = true;
            Thread.Sleep(750); // add pause to be more human in interaction
            return isCaptchaNeeded;
        }

        protected virtual void IterateItems(IWebDriver driver, TravisSearchProcess parameters, List<ITravisSearchAction> postcommon)
        {
            if (driver == null) throw new ArgumentNullException(nameof(driver));
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            if (postcommon == null) throw new ArgumentNullException(nameof(postcommon));
            if (ExecutionCancelled) return;
            Items.ForEach(item =>
            {
                if (item.Court == "Precinct One") item.Court = "Precinct 1";
                if (item.Court == "Precinct Two") item.Court = "Precinct 2";
                if (item.Court == "Precinct Three") item.Court = "Precinct 3";
                if (item.Court == "Precinct Four") item.Court = "Precinct 4";
                if (item.Court == "Precinct Five") item.Court = "Precinct 5";
            });
            CaseStyles.ForEach(c =>
            {
                var item = Items.Find(x =>
                {
                    if (x.CaseNumber.Equals(c.CaseNumber, StringComparison.OrdinalIgnoreCase)) return true;
                    return x.CaseStyle.Equals(x.CaseStyle, StringComparison.OrdinalIgnoreCase);
                });
                if (item != null &&
                    !string.IsNullOrEmpty(c.Plaintiff) &&
                    !string.IsNullOrEmpty(c.PartyName) &&
                    !string.IsNullOrEmpty(c.Address))
                {
                    c.CaseNumber = item.CaseNumber;
                    item.Court = c.Court;
                    item.Plaintiff = c.Plaintiff;
                    item.PartyName = c.PartyName;
                    if (item.Court == "Precinct One") item.Court = "Precinct 1";
                    if (item.Court == "Precinct Two") item.Court = "Precinct 2";
                    if (item.Court == "Precinct Three") item.Court = "Precinct 3";
                    if (item.Court == "Precinct Four") item.Court = "Precinct 4";
                    if (item.Court == "Precinct Five") item.Court = "Precinct 5";
                    AppendPerson(item);
                }
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
                else
                {
                    if (string.IsNullOrWhiteSpace(p.Name))
                    {
                        p.Name = TryMapName(i);
                    }
                }
            });
        }

        private string TryMapName(string caseNumber)
        {
            var style = CaseStyles.Find(x => x.CaseNumber.Equals(caseNumber, StringComparison.OrdinalIgnoreCase));
            if (style == null) return string.Empty;
            return style.PartyName;
        }

        private void Populate(ITravisSearchAction a, IWebDriver driver, TravisSearchProcess parameters, int locationId, string uri = "")
        {
            Debug.WriteIf(!string.IsNullOrEmpty(uri), "uri is provided");
            a.Driver = driver;
            a.Parameters = parameters;
            if (a is TravisRequestCaptcha captcha) { captcha.PromptUser = UserPrompt; }
            if (a is TravisFetchCaseItems items) { items.PauseForPage = false; }
            if (a is TravisSetupParameters prms) { prms.CourtLocationId = locationId; }
            if (a is TravisSetupAlternateParameters altprms) { altprms.CourtLocationId = locationId; }
        }

        [ExcludeFromCodeCoverage]
        private bool UserPrompt()
        {
            if (!DisplayDialogue)
            {
                Thread.Sleep(100);
                return true;
            }
            var response = DialogResult.None;
            while (response != DialogResult.OK)
            {
                response = MessageBox.Show(Rx.UI_CAPTCHA_DESCRIPTION, Rx.UI_CAPTCHA_TITLE, MessageBoxButtons.OKCancel);
                if (response == DialogResult.Cancel) break;
            }
            return response == DialogResult.OK;
        }

        private void AppendPerson(CaseItemDto dto)
        {
            var person = dto.FromDto();
            var address = GetAddress(CaseStyles.Find(c => c.CaseStyle.Equals(dto.CaseStyle, StringComparison.OrdinalIgnoreCase)));
            if (address != null && address.Any())
            {
                person.UpdateAddress(address);
            }
            People.Add(person);
        }

        private static List<string> GetAddress(TravisCaseStyleDto dto)
        {
            var pipe = "|";
            var doublepipe = "||";
            if (dto == null || string.IsNullOrEmpty(dto.Address)) return null;
            var data = dto.Address;
            while (data.Contains(doublepipe)) { data = data.Replace(doublepipe, pipe); }
            return data.Split('|').ToList();
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

        [ExcludeFromCodeCoverage]
        private static List<TravisCaseStyleDto> GetMapping(string json)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<List<TravisCaseStyleDto>>(json);
                if (data == null) return new List<TravisCaseStyleDto>();
                return data;
            }
            catch (Exception)
            {
                return new List<TravisCaseStyleDto>();
            }
        }

        private string GetExcelFileName()
        {
            var folder = ExcelDirectoyName();
            var name = TravisSearchProcess.GetCourtName(CourtType);
            var fmt = $"TRAVIS_{name}_{GetDateString(StartDate)}_{GetDateString(EndingDate)}";
            var fullName = Path.Combine(folder, $"{fmt}.xlsx");
            var idx = 1;
            while (File.Exists(fullName))
            {
                fullName = Path.Combine(folder, $"{fmt}_{idx:D4}.xlsx");
                idx++;
            }
            var writer = new ExcelWriter();
            var content = writer.ConvertToPersonTable(addressList: People, worksheetName: "addresses", websiteId: websiteMappingId);
            var courtlist = People.Select(p =>
            {
                if (string.IsNullOrEmpty(p.Court)) return string.Empty;
                var find = TravisCourtLookupService.GetAddress(name, p.Court);
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
        private static readonly CultureInfo Culture = CultureInfo.CurrentCulture;
        private const int websiteMappingId = 70;
    }
}
