using OpenQA.Selenium;
using System.Linq;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Addressing
{
    public class FindCaseDataPoint : FindDefendantBase
    {


        public override bool CanFind { get; set; }

        public override void Find(IWebDriver driver, HLinkDataRow linkData)
        {
            CanFind = false;
            var dto = DataPointLocatorDto.GetDto("tarrantCountyDataPoint");
            var search = dto.DataPoints.First(x => x.Name.Equals("CaseStyle"));
            //var helper = new ElementAssertion(driver);
            //helper.Navigate(linkData.Uri);
            //driver.WaitForNavigation();
            var element = driver.FindElement(By.XPath(search.Xpath));
            search.Result = element.Text;
            linkData.PageHtml = Newtonsoft.Json.JsonConvert.SerializeObject(dto);

        }
    }
}
