// FindDefendantNavigation
using OpenQA.Selenium;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Addressing
{
    public class FindDefendantNavigation : FindDefendantBase
    {
        public override bool CanFind { get; set; }

        public override void Find(IWebDriver driver, HLinkDataRow linkData)
        {
            CanFind = false;
            var helper = new ElementAssertion(driver);
            helper.Navigate(linkData.Uri);
            driver.WaitForNavigation();
            // get criminal hyperlink
            // //a[contains(text(),'Criminal')]
            var criminalLink = TryFindElement(driver, By.XPath("//a[@class = 'ssBlackNavBarHyperlink'][contains(text(),'Criminal')]"));
            if (criminalLink != null)
            {
                var elementCaseName = TryFindElement(driver, By.XPath("/html/body/table[3]/tbody/tr/td[1]/b"));
                if (elementCaseName != null)
                {
                    linkData.CriminalCaseStyle = elementCaseName.Text;
                    linkData.IsCriminal = true;
                } 
            }
            linkData.PageHtml = 
                GetTable(driver, By.XPath(@"//div[contains(text(),'Party Information')]"));
            
        }

        private string GetTable(IWebDriver driver, By by)
        {
            try
            {
                var dv = driver.FindElement(by);
                var parent = dv.FindElement(By.XPath(".."));
                while(parent.TagName != "table")
                {
                    parent = parent.FindElement(By.XPath(".."));
                }
                return parent.GetAttribute("outerHTML");
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}

