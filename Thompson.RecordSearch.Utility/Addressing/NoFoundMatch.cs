// NoFoundMatch
using OpenQA.Selenium;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Addressing
{
    public class NoFoundMatch : FindDefendantBase
    {
        private const string _noMatch = @"No Match Found<br/>Not Matched 00000";
        public override bool CanFind { get; set; }

        public override void Find(IWebDriver driver, HLinkDataRow linkData)
        {
            if (driver == null)
            {
                throw new System.ArgumentNullException(nameof(driver));
            }

            if (linkData == null)
            {
                throw new System.ArgumentNullException(nameof(linkData));
            }
            // driver.FindElement(By.XPath("//th[contains(text(),'Principal')]"))
            CanFind = true;
            linkData.Address = _noMatch;
        }
        internal static string GetNoMatch(string currentAddress)
        {
            return string.IsNullOrEmpty(currentAddress) ? _noMatch : currentAddress;
        }
    }
}
