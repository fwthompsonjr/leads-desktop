using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Common;
using LegalLead.PublicData.Search.Interfaces;
using OpenQA.Selenium;
using Thompson.RecordSearch.Utility.Classes;

namespace LegalLead.PublicData.Search.Util
{
    public class TravisConversionAction : ICountySearchAction
    {
        public TravisConversionAction(ITravisSearchAction search) :
            this(search.OrderId, search.Driver, search.Parameters)
        { }

        public TravisConversionAction(int orderId, IWebDriver driver, TravisSearchProcess parameters)
        {
            OrderId = orderId;
            Driver = driver;
            Parameters = parameters.ToDallasSearch();
        }

        public int OrderId { get; private set; }

        public IWebDriver Driver { get; set; }
        public DallasSearchProcess Parameters { get; set; }
        public IWebInteractive Interactive { get; set; }

        public object Execute()
        {
            return string.Empty;
        }
    }
}
