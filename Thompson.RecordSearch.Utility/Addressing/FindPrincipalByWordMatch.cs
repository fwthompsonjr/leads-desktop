// FindPrincipalByWordMatch
using System;
using System.Linq;
using OpenQA.Selenium;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Addressing
{
    public class FindPrincipalByWordMatch : FindDefendantBase
    {
        public override bool CanFind { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types",
            Justification = "Exception thrown from this method will stop automation.")]
        public override void Find(IWebDriver driver, HLinkDataRow linkData)
        {
            if (driver == null) throw new System.ArgumentNullException(nameof(driver));
            if (linkData == null) throw new System.ArgumentNullException(nameof(linkData));
            var searchType = "Principal";
            CanFind = false;
            var tdName = TryFindElement(driver, By.XPath(
                String.Format(@"//th[contains(text(),'{0}')]", searchType)));
            // this instance can find
            if (tdName == null) return;

            var parent = tdName.FindElement(By.XPath(".."));
            var rowLabel = parent.FindElements(By.TagName("th"))[1];
            linkData.Defendant = rowLabel.GetAttribute("innerText");
            CanFind = true;
            linkData.Address = parent.Text;
            try
            {

                // get row index of this element ... and then go one row beyond...
                var ridx = parent.GetAttribute("rowIndex");
                var table = parent.FindElement(By.XPath(".."));
                var trCol = table.FindElements(By.TagName("tr")).ToList();
                if (!int.TryParse(ridx, out int r)) return;
                MapElementAddress(linkData, rowLabel, table, trCol, r, searchType.ToLower());
            }
            catch (Exception)
            {

            }
        }

    }
}
