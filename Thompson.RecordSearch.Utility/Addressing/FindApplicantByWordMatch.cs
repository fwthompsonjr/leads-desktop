// FindApplicantByWordMatch
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Addressing
{
    public class FindApplicantByWordMatch : FindDefendantBase
    {
        public override bool CanFind { get; set; }

        public override void Find(IWebDriver driver, HLinkDataRow linkData)
        {
            var searchType = "Applicant";
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
