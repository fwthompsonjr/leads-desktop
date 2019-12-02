using OpenQA.Selenium;
using System.Linq;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Interfaces;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Web
{

    public abstract class ElementActionBase : IElementActionBase
    {

        public string OuterHtml { get; set; }

        public virtual ElementAssertion GetAssertion { get; set; }

        public virtual IWebDriver GetWeb { get; set; }

        public virtual string ActionName { get; }

        public abstract void Act(Step step);

        public WebNavigationParameter GetSettings(int index)
        {

            var websites = new SettingsManager().GetNavigation();

            var siteData = websites.First(x => x.Id == index);
            return siteData;
        }

        protected By GetSelector(Step item)
        {
            if (item.Locator.Find.Equals("css"))
                return By.CssSelector(item.Locator.Query);
            if (item.Locator.Find.Equals("xpath"))
                return By.XPath(item.Locator.Query);
            return null;
        }
    }
}
