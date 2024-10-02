using LegalLead.PublicData.Search.Classes;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
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
            DisplayDialogue = displayDialogue;
            var container = DallasActionContainer.GetContainer;
            var collection = container.GetAllInstances<IDallasAction>().ToList();
            collection.Sort((a,b) => a.OrderId.CompareTo(b.OrderId));
            ActionItems.AddRange(collection);
        }
        public List<PersonAddress> People { get; private set; } = new List<PersonAddress>();
        public List<DallasCaseItemDto> Items { get; private set; } = new List<DallasCaseItemDto>();
        protected bool DisplayDialogue { get; set; }
        private readonly List<IDallasAction> ActionItems = new List<IDallasAction>();
        public override WebFetchResult Fetch()
        {
            var postsearchtypes = new List<Type> { typeof(DallasFetchCaseStyle) };
            var driver = new FireFoxProvider().GetWebDriver();
            var parameters = new DallasAttendedProcess();
            var dates = DallasAttendedProcess.GetBusinessDays(StartDate, EndingDate);
            var common = ActionItems.FindAll(a => !postsearchtypes.Contains(a.GetType()));
            var result = new WebFetchResult();
            dates.ForEach(d =>
            {
                parameters.Search(d, d, "SearchType");
                common.ForEach(a => {
                    Populate(a, driver, parameters);
                    var response = a.Execute();
                    if (a is DallasFetchCaseDetail _ && response is string cases)
                    {
                        Items.AddRange(GetData(cases));
                    }
                });
            });
            return result;
        }

        private void Populate(IDallasAction a, IWebDriver driver, DallasAttendedProcess parameters)
        {
            a.Driver = driver;
            a.Parameters = parameters;
            if (a is DallasRequestCaptcha captcha) { captcha.PromptUser = UserPrompt; }
        }
        private void UserPrompt()
        {
            if (!DisplayDialogue)
            {
                Thread.Sleep(100);
                return;
            }
            var response = DialogResult.None;
            while (response != DialogResult.OK)
            {
                response = MessageBox.Show(Rx.UI_CAPTCHA_DESCRIPTION, Rx.UI_CAPTCHA_TITLE, MessageBoxButtons.OKCancel);
            }
        }

        private static List<DallasCaseItemDto> GetData(string json)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<List<DallasCaseItemDto>>(json);
                if (data != null) return new List<DallasCaseItemDto>();
                return data;
            }
            catch (Exception)
            {
                return new List<DallasCaseItemDto>();
            }
        }
    }
}
