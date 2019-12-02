using System;
using System.Linq;
using OpenQA.Selenium;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Addressing
{
    public class FindDefendantByWordMatch : FindDefendantBase
    {
        public override bool CanFind { get; set; }

        public override void Find(IWebDriver driver, HLinkDataRow linkData)
        {
            // driver.FindElement(By.XPath("//th[contains(text(),'Principal')]"))
            // todo: CV-2019-02188-JP... exmple of a case with multiple defendants...
            // create a lookup for multiples
            CanFind = false;
            var tdName = TryFindElement(driver, By.XPath(@"//th[contains(text(),'Defendant')]"));
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

                var nextTh = table.FindElements(By.TagName("th")).ToList().FirstOrDefault(x => x.Location.Y > rowLabel.Location.Y);
                var mxRowIndex = nextTh == null ? r + 1: Convert.ToInt32(nextTh.FindElement(By.XPath("..")).GetAttribute("rowIndex"));
                while (r <= mxRowIndex)
                {
                    var currentRow = trCol[r];
                    var tdElements = currentRow.FindElements(By.TagName("td")).ToList();
                    tdElements = tdElements.FindAll(x => x.Location.X >= rowLabel.Location.X & x.Location.X < (rowLabel.Location.X + rowLabel.Size.Width));
                    linkData.Address = GetAddress(r, tdElements);
                    if (!string.IsNullOrEmpty(linkData.Address)) break;
                    r = r + 1;
                }
                // parent = GetAddressRow(parent, trCol); // put this row-index into config... it can change
                // linkData.Address = new StringBuilder(parent.Text).Replace(Environment.NewLine, "<br/>").ToString();
            }
            catch (Exception)
            {

            }
        }

    }
}
