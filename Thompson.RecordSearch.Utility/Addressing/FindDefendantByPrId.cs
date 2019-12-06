using System;
using System.Text;
using OpenQA.Selenium;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Addressing
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types",
        Justification = "Exception thrown from this method will stop automation.")]
    public class FindDefendantByPrId : FindDefendantBase
    {
        public override bool CanFind { get; set; }

        public override void Find(IWebDriver driver, HLinkDataRow linkData)
        {
            if (driver == null) throw new System.ArgumentNullException(nameof(driver));
            if (linkData == null) throw new System.ArgumentNullException(nameof(linkData));
            CanFind = false;
            var tdName = TryFindElement(driver, By.XPath(@"//*[@id='PIr11']"));
            // this instance can find
            if (tdName == null) return;

            linkData.Defendant = tdName.GetAttribute("innerText");
            var parent = tdName.FindElement(By.XPath(".."));
            var rowLabel = parent.FindElements(By.TagName("th"))[0];
            if(rowLabel.Text.Trim()
                .ToLower(System.Globalization.CultureInfo.CurrentCulture) != "defendant")
            {
                return;
            }
            CanFind = true;
            linkData.Address = parent.Text;
            try
            {

                // get row index of this element ... and then go one row beyond...
                var ridx = parent.GetAttribute("rowIndex");
                var table = parent.FindElement(By.XPath(".."));
                var trCol = table.FindElements(By.TagName("tr"));
                if (!int.TryParse(ridx, out int r)) return;
                parent = GetAddressRow(parent, trCol); // put this row-index into config... it can change
                linkData.Address = new StringBuilder(parent.Text).Replace(Environment.NewLine, "<br/>").ToString();
            }
            catch (Exception)
            {

            }
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

