﻿// FindRespondentByWordMatch
using OpenQA.Selenium;
using System;
using System.Globalization;
using System.Linq;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Addressing
{
    public class FindRespondentByWordMatch : FindDefendantBase
    {
        public override bool CanFind { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types",
            Justification = "Exception thrown from this method will stop automation.")]
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

            var searchType = "Respondent";
            CanFind = false;
            var tdName = TryFindElement(driver, By.XPath(
                String.Format(IndexKeyNames.ThContainsText, searchType)));
            // this instance can find
            if (tdName == null)
            {
                return;
            }

            var parent = tdName.FindElement(By.XPath(IndexKeyNames.ParentElement));
            var rowLabel = parent.FindElements(By.TagName(IndexKeyNames.ThElement))[1];
            linkData.Defendant = rowLabel.GetAttribute(IndexKeyNames.InnerText);
            CanFind = true;
            linkData.Address = parent.Text;
            try
            {

                // get row index of this element ... and then go one row beyond...
                var ridx = parent.GetAttribute(IndexKeyNames.RowIndex);
                var table = parent.FindElement(By.XPath(IndexKeyNames.ParentElement));
                var trCol = table.FindElements(By.TagName(IndexKeyNames.TrElement)).ToList();
                if (!int.TryParse(ridx, out int r))
                {
                    return;
                }

                MapElementAddress(linkData, rowLabel, table, trCol, r, searchType.ToLower(CultureInfo.CurrentCulture));
            }
            catch (Exception)
            {

            }
        }


    }
}
