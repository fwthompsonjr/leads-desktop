// FindDefendantNavigation
using OpenQA.Selenium;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Addressing
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types",
        Justification = "Exception thrown from this method will stop automation.")]
    public class FindDefendantNavigation : FindDefendantBase
    {
        public override bool CanFind { get; set; }

        public override void Find(IWebDriver driver, HLinkDataRow linkData)
        {
            if (driver == null) throw new System.ArgumentNullException(nameof(driver));
            if (linkData == null) throw new System.ArgumentNullException(nameof(linkData));
            CanFind = false;
            var helper = new ElementAssertion(driver);
            helper.Navigate(linkData.WebAddress);
            driver.WaitForNavigation();
            // get criminal hyperlink
            // //a[contains(text(),'Criminal')]
            var criminalLink = TryFindElement(driver, By.XPath(CommonKeyIndexes.CriminalLinkXpath)); 
            var elementCaseName = TryFindElement(driver, By.XPath(CommonKeyIndexes.CaseStlyeBoldXpath)); 
            if (criminalLink != null)
            {
                if (elementCaseName != null)
                {
                    linkData.CriminalCaseStyle = elementCaseName.Text;
                }
                linkData.IsCriminal = true;
            }
            if(linkData.IsJustice && elementCaseName != null)
            {
                linkData.CriminalCaseStyle = elementCaseName.Text;
            }
            linkData.PageHtml = 
                GetTable(driver, By.XPath(@"//div[contains(text(),'Party Information')]"));
            
        }

        private string GetTable(IWebDriver driver, By by)
        {
            try
            {
                var dv = driver.FindElement(by);
                var parent = dv.FindElement(By.XPath(IndexKeyNames.ParentElement));
                while(parent.TagName != "table")
                {
                    parent = parent.FindElement(By.XPath(IndexKeyNames.ParentElement));
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

