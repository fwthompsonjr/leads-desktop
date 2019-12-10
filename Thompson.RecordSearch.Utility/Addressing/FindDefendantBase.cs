using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Addressing
{
    public abstract class FindDefendantBase
    {

        /// <summary>
        /// Gets or sets a value indicating whether this instance can find.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can find; otherwise, <c>false</c>.
        /// </value>
        public abstract bool CanFind { get; set; }

        /// <summary>
        /// Finds the specified defendant record from web page source.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="webInteractive">The web interactive.</param>
        /// <param name="linkData">The link data.</param>
        public abstract void Find(IWebDriver driver, HLinkDataRow linkData);



        /// <summary>
        /// Locates the address element from the case-detail drill down page.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="trCol">The tr col.</param>
        /// <returns></returns>
        protected static IWebElement GetAddressRow(IWebElement parent, System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> trCol)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (trCol == null) throw new ArgumentNullException(nameof(trCol));
            int colIndex = 3;
            parent = trCol[colIndex];
            var parentTxt = string.IsNullOrEmpty(parent.Text) ? string.Empty : parent.Text;
            if (string.IsNullOrEmpty(parentTxt)) parent = trCol[colIndex - 1];
            return parent;
        }



        /// <summary>
        /// Tries the find element on a specfic web page using the By condition supplied.
        /// </summary>
        /// <param name="parent">The parent web browser instance.</param>
        /// <param name="by">The by condition used to locate the element</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        protected static IWebElement TryFindElement(IWebDriver parent, By by)
        {
            try
            {
                if (parent == null) return null;
                return parent.FindElement(by);
            }
            catch (Exception)
            {
                return null;
            }
        }

        protected static string GetAddress(List<IWebElement> elements, string keyword = "defendant")
        {
            if (elements == null) throw new ArgumentNullException(nameof(elements));
            if (string.IsNullOrEmpty(keyword)) { keyword = "defendant"; }
            const StringComparison currentCase = StringComparison.CurrentCultureIgnoreCase;
            var cultureInfo = System.Globalization.CultureInfo.CurrentCulture;
            var address = string.Empty;
            var addressHtml = string.Empty;
            var findKey = keyword.ToLower(cultureInfo);
            if (!elements.Any()) return string.Empty;
            var rowIndex = 0;
            while (address.ToLower(cultureInfo).StartsWith(findKey, currentCase) | string.IsNullOrEmpty(address))
            {
                if (rowIndex > elements.Count - 1) break;
                var currentRow = elements[rowIndex];
                var cells = currentRow.FindElements(By.TagName("td")).ToList();
                var firstTd = cells.Any() ? cells[0] : null;
                addressHtml = firstTd != null ? firstTd.GetAttribute("innerHTML").Trim() : string.Empty;
                address = currentRow.GetAttribute("innerText")
                    .Replace(Environment.NewLine, "<br/>").Trim();
                rowIndex += 1;
                var headings = currentRow.FindElements(By.TagName("th"));
                
                if (headings != null && headings.Count > 1)
                {
                    if (!address.ToLower(cultureInfo).StartsWith(findKey, currentCase)) {
                        address = string.Empty;
                        break; }
                }
                if (firstTd != null &&
                    string.IsNullOrEmpty(firstTd.GetAttribute("headers")))
                {
                    address = string.Empty;
                }
            }
            if (string.IsNullOrEmpty(addressHtml)) return address;
            // custom clean ups for collin-county
            const string noBr = @"<nobr>";
            const string br = @"<br/>";
            if (addressHtml.IndexOf(noBr, currentCase) < 0)
            {
                return address;
            }
            var endOfLine = addressHtml.IndexOf(noBr, currentCase);
            addressHtml = addressHtml.Substring(0, endOfLine);
            addressHtml = new StringBuilder(addressHtml)
                .Replace("&nbsp;", " ")
                .Replace("<br>", br)
                .Replace(Environment.NewLine, br)
                .Replace(br, "|").ToString();
            return addressHtml;
        }

        protected static void MapElementAddress(HLinkDataRow linkData,
            IWebElement rowLabel,
            IWebElement table,
            List<IWebElement> trCol,
            int r,
            string searchTitle)
        {
            if (linkData == null) throw new ArgumentNullException(nameof(linkData));
            if (rowLabel == null) throw new ArgumentNullException(nameof(rowLabel));
            if (table == null) throw new ArgumentNullException(nameof(table));
            if (trCol == null) throw new ArgumentNullException(nameof(trCol));
            var nextTh = table.FindElements(By.TagName("th")).ToList().FirstOrDefault(x => x.Location.Y > rowLabel.Location.Y);
            var mxRowIndex = nextTh == null ? r : Convert.ToInt32(nextTh.FindElement(By.XPath("..")).GetAttribute("rowIndex"),
                CultureInfo.CurrentCulture.NumberFormat);
            while (r <= mxRowIndex)
            {
                var currentRow = trCol[r];
                var tdElements = currentRow.FindElements(By.TagName("td")).ToList();
                tdElements = tdElements.FindAll(x => x.Location.X >= rowLabel.Location.X & x.Location.X < (rowLabel.Location.X + rowLabel.Size.Width));
                linkData.Address = GetAddress(tdElements, searchTitle);
                if (!string.IsNullOrEmpty(linkData.Address)) break;
                r += 1;
            }
            linkData.Address = NoFoundMatch.GetNoMatch(linkData.Address);
        }
    }
}
