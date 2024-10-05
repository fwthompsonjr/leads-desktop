using LegalLead.PublicData.Search.Classes;
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
            var container = DallasActionContainer.GetContainer;
            var collection = container.GetAllInstances<IDallasAction>().ToList();
            collection.Sort((a, b) => a.OrderId.CompareTo(b.OrderId));
            ActionItems.AddRange(collection);
        }

        public List<PersonAddress> People { get; private set; } = new List<PersonAddress>();
        public List<DallasCaseItemDto> Items { get; private set; } = new List<DallasCaseItemDto>();
        protected bool ExecutionCancelled { get; set; }
        protected bool DisplayDialogue { get; set; }
        protected string CourtType { get; set; }
        private readonly List<IDallasAction> ActionItems = new List<IDallasAction>();
        public override WebFetchResult Fetch()
        {
            var postsearchtypes = new List<Type> { typeof(DallasFetchCaseStyle) };
            var driver = GetDriver();
            var parameters = new DallasAttendedProcess();
            var dates = DallasAttendedProcess.GetBusinessDays(StartDate, EndingDate);
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
        protected virtual void Iterate(IWebDriver driver, DallasAttendedProcess parameters, List<DateTime> dates, List<IDallasAction> common, List<IDallasAction> postcommon)
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
                Console.WriteLine(ex);
            }
            finally
            {
                driver?.Quit();
            }

        }

        protected virtual void IterateDateRange(IWebDriver driver, DallasAttendedProcess parameters, List<DateTime> dates, List<IDallasAction> common)
        {
            if (driver == null) throw new ArgumentNullException(nameof(driver));
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            if (dates == null) throw new ArgumentNullException(nameof(dates));
            if (common == null) throw new ArgumentNullException(nameof(common));
            var count = common.Count - 1;
            dates.ForEach(d =>
            {
                parameters.Search(d, d, CourtType);
                common.ForEach(a =>
                {
                    if (!ExecutionCancelled)
                    {
                        Populate(a, driver, parameters);
                        var response = a.Execute();
                        if (a is DallasFetchCaseDetail _ && response is string cases) Items.AddRange(GetData(cases));
                        if (a is DallasRequestCaptcha _ && response is bool canExecute && !canExecute) ExecutionCancelled = true;
                        if (common.IndexOf(a) != count) Thread.Sleep(750); // add pause to be more human in interaction
                    }
                });
            });
            parameters.Search(dates[0], dates[dates.Count - 1], CourtType);
        }


        protected virtual void IterateItems(IWebDriver driver, DallasAttendedProcess parameters, List<IDallasAction> postcommon)
        {
            if (driver == null) throw new ArgumentNullException(nameof(driver));
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            if (postcommon == null) throw new ArgumentNullException(nameof(postcommon));
            if (ExecutionCancelled) return;
            Items.ForEach(i =>
            {
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

        private void Populate(IDallasAction a, IWebDriver driver, DallasAttendedProcess parameters, string uri = "")
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

        private void AppendPerson(DallasCaseItemDto dto)
        {
            var person = dto.FromDto();
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
        private static List<DallasCaseItemDto> GetData(string json)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<List<DallasCaseItemDto>>(json);
                if (data == null) return new List<DallasCaseItemDto>();
                return data;
            }
            catch (Exception)
            {
                return new List<DallasCaseItemDto>();
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
            var name = DallasAttendedProcess.GetCourtName(CourtType);
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
            content.TransferColumn("County", "fname");
            content.TransferColumn("CourtAddress", "lname");

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
            return date.ToString(fmt);

        }

        private static string ExcelDirectoyName()
        {
            var appFolder =
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var xmlFolder = Path.Combine(appFolder, "data");
            if (!Directory.Exists(xmlFolder)) Directory.CreateDirectory(xmlFolder);
            return xmlFolder;
        }

    }
}
