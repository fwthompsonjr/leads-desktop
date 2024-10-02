using LegalLead.PublicData.Search.Classes;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.DriverFactory;
using Thompson.RecordSearch.Utility.Dto;
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
            result.PeopleList = People;
            return result;
        }

        public virtual IWebDriver GetDriver(bool headless = false)
        {
            return new FireFoxProvider().GetWebDriver(headless);
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
        }

        private void Populate(IDallasAction a, IWebDriver driver, DallasAttendedProcess parameters, string uri = "")
        {
            a.Driver = driver;
            a.Parameters = parameters;
            if (a is DallasRequestCaptcha captcha) { captcha.PromptUser = UserPrompt; }
            if (!string.IsNullOrEmpty(uri) && a is DallasFetchCaseStyle style) { style.PageAddress = uri; }
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
            }
            return response == DialogResult.OK;
        }

        private void AppendPerson(DallasCaseItemDto dto)
        {
            var person = new PersonAddress
            {
                Court = dto.Court,
                CaseNumber = dto.CaseNumber,
                CaseStyle = dto.CaseStyle,
                DateFiled = dto.FileDate,
                Status = dto.CaseStatus,
                Name = dto.PartyName,
                Plantiff = dto.Plaintiff,
                Zip = "00000",
                Address1 = "000 No Street",
                Address3 = "Not, NA"
            };
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
    }
}
