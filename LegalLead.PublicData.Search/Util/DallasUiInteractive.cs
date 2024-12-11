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
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            StartDate = FetchKeyedData(parameters.Keys, "StartDate");
            EndingDate = FetchKeyedData(parameters.Keys, "EndDate");
            CourtType = FetchKeyedItem(parameters.Keys, "CourtType");
            DisplayDialogue = displayDialogue;
            var container = ActionDallasContainer.GetContainer;
            var collection = container.GetAllInstances<ICountySearchAction>().ToList();
            collection.Sort((a, b) => a.OrderId.CompareTo(b.OrderId));
            ActionItems.AddRange(collection);
        }

        public List<PersonAddress> People { get; private set; } = new List<PersonAddress>();
        public List<CaseItemDto> Items { get; private set; } = new List<CaseItemDto>();
        protected List<DallasCaseStyleDto> CaseStyles { get; private set; } = new List<DallasCaseStyleDto>();
        protected bool ExecutionCancelled { get; set; }
        protected bool DisplayDialogue { get; set; }
        protected string CourtType { get; set; }
        private readonly List<ICountySearchAction> ActionItems = new();
        public override WebFetchResult Fetch()
        {
            try
            {
                using var hider = new HideProcessWindowHelper();
                var postsearchtypes = new List<Type> { typeof(DallasFetchCaseStyle) };
                var driver = GetDriver(DriverReadHeadless);
                var parameters = new DallasSearchProcess();
                var dates = DallasSearchProcess.GetBusinessDays(StartDate, EndingDate);
                var common = ActionItems.FindAll(a => !postsearchtypes.Contains(a.GetType()));
                var postcommon = ActionItems.FindAll(a => postsearchtypes.Contains(a.GetType()));
                var result = new WebFetchResult();
                Iterate(driver, parameters, dates, common, postcommon);
                if (ExecutionCancelled || People.Count == 0) return result;
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
                Console.WriteLine("Searching for records on date: {0:d}", d);
                parameters.SetSearchParameters(d, d, CourtType);
                common.ForEach(a =>
                {
                    isCaptchaNeeded = IterateCommonActions(isCaptchaNeeded, driver, parameters, common, a);
                });
            });
            parameters.SetSearchParameters(dates[0], dates[^1], CourtType);
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
            var response = a.Execute();
            if (a is BaseDallasSearchAction postaction) { postaction.MaskUserName(); }
            if (a is DallasAuthenicateSubmit _ && response is bool captchaCompleted && captchaCompleted) isCaptchaNeeded = false;
            if (a is DallasFetchCaseDetail _ && response is string cases) Items.AddRange(GetData(cases));
            if (a is DallasRequestCaptcha _ && response is bool canExecute && !canExecute) ExecutionCancelled = true;
            if (common.IndexOf(a) != count) Thread.Sleep(750); // add pause to be more human in interaction
            return isCaptchaNeeded;
        }

        protected virtual void IterateItems(IWebDriver driver, DallasSearchProcess parameters, List<ICountySearchAction> postcommon)
        {
            if (driver == null) throw new ArgumentNullException(nameof(driver));
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            if (postcommon == null) throw new ArgumentNullException(nameof(postcommon));
            if (ExecutionCancelled) return;
            var count = Items.Count;
            Items.ForEach(i =>
            {
                var c = Items.IndexOf(i) + 1;
                this.EchoProgess(0, count, c, $"Reading item {c} of {count}.", true);
                postcommon.ForEach(a =>
                {
                    Populate(a, driver, parameters, i.Href);
                    var response = a.Execute();
                    if (a is DallasFetchCaseStyle _ && response is string cases)
                    {
                        var info = GetStyle(cases);
                        if (info != null)
                        {
                            i.CaseStyle = info.CaseStyle;
                            i.Plaintiff = info.Plaintiff;
                            if (!string.IsNullOrWhiteSpace(info.Address)) { CaseStyles.Add(info); }
                            AppendPerson(i);
                        }
                    }
                });
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
        private static DallasCaseStyleDto GetStyle(string json)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<DallasCaseStyleDto>(json);
                if (data == null) return new DallasCaseStyleDto();
                return data;
            }
            catch (Exception)
            {
                return new DallasCaseStyleDto();
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
