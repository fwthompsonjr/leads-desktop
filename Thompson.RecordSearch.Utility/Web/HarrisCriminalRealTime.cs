using Harris.Criminal.Db.Downloads;
using HtmlAgilityPack;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Dto;

namespace Thompson.RecordSearch.Utility.Web
{
    public class HarrisCriminalRealTime : HarrisCriminalData
    {

        private const string divResults = "#ctl00_ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder2_ContentPlaceHolder2_resultsPanel";
        private const string tbPager = divResults + " table.PagerContainerTable";
        private const string pagerLink = "a.PagerHyperlinkStyle";
        private const string recordCount = "td.PagerInfoCell";
        private const string dataTable = divResults + " table.resultHeader.contentwidth";
        private const string searchResult = "//*[@id=\"ctl00_ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder2_ContentPlaceHolder2_pnlSearchResult\"]";
        private const string searchLinks = searchResult + "/table[1]/tbody/tr[4]/td/table/tbody/tr/td[2]/a";
        private const string resultGrid = "//table[@class='cd_resultgrid'][@width='100%']";

        public void IteratePages(IWebDriver driver)
        {

            if (driver == null)
            {
                driver = GetDriver();
            }
            TheDriver = driver;
            var pager = GetElementsOrFail(By.CssSelector(tbPager)).FirstOrDefault();
            // get the expected record count
            int count = GetRecordCount(pager, By.CssSelector(recordCount));
            var cases = new List<HarrisCriminalStyleDto>();
            var details = new List<HarrisCriminalDto>();
            var runDate = DateTime.Now;
            for (int r = 1; r <= count; r++)
            {
                var currentWindow = driver.CurrentWindowHandle;
                if (r != 1)
                {
                    // find the pager element
                    pager = GetElementsOrFail(By.CssSelector(tbPager)).FirstOrDefault();
                    ClickPageHyperlink(pager.FindElements(By.CssSelector(pagerLink)).ToList(), r);
                }
                var table = GetElementOrFail(By.CssSelector(dataTable)).GetAttribute("outerHTML");
                var doc = new HtmlDocument();
                doc.LoadHtml(table);
                var parentNode = doc.DocumentNode.FirstChild;
                var nodes = parentNode.SelectNodes("//tr").Cast<HtmlNode>().ToList();
                // read case header
                nodes.ForEach(n => 
                {
                    if (nodes.IndexOf(n) != 0)
                    {
                        var cells = n.SelectNodes("td").Cast<HtmlNode>().ToList();
                        AppendCases(cases, cells);
                    }
                });

                // read case detail
                var searchable = GetElementsOrFail(By.XPath(searchLinks));
                // read case details
                searchable.ForEach(s =>
                {
                    if (searchable.IndexOf(s) != 0)
                    {
                        driver.ClickAndOrSetText(s);
                        var newWindow = driver.WindowHandles.ToList()
                            .FirstOrDefault(h => !h.Equals(currentWindow, StringComparison.OrdinalIgnoreCase));
                        driver.SwitchTo().Window(newWindow);
                        var cdResults = GetElementsOrFail(By.XPath(resultGrid))
                            .Select(x => ToHtmlDocument(x.GetAttribute("outerHTML")))
                            .ToList()
                            .FindAll(f => f != null);
                        AppendDetail(details, cdResults, runDate);
                        driver.Close();
                        driver.SwitchTo().Window(currentWindow);
                    }
                });

            }
        }

        private static void AppendDetail(List<HarrisCriminalDto> details, List<HtmlDocument> cdResults, DateTime runDate)
        {
            if (details == null | cdResults == null)
            {
                return;
            }
            if (cdResults.Count < 3)
            {
                return;
            }
            const string tbl = "//table";
            const string tr = "//tr";
            var rows = new PopUpTable
            {
                Index = details.Count + 1,
                RunDate = runDate,
                CauseSummary = cdResults[0].DocumentNode?.FirstChild?.SelectNodes(tr)?.ToList(),
                DefendantAddress = cdResults[1]?.DocumentNode?.FirstChild?.SelectNodes(tr)?.ToList(),
                DefendantBio = cdResults[1]?.DocumentNode?.FirstChild?.SelectNodes(tbl)[0]?.SelectNodes(tr)?.ToList(),
                DefendantMetrics = cdResults[1]?.DocumentNode?.FirstChild?.SelectNodes(tbl)[1]?.SelectNodes(tr)?.ToList(),
                Court = cdResults[2]?.DocumentNode?.FirstChild?.SelectNodes(tr)?.ToList()
            };
            var dto = rows.CriminalDto();
            if (dto == null) return;
            details.Add(dto);
        }

        private static HtmlDocument ToHtmlDocument(string table)
        {
            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(table);
                return doc;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                return null;
            }
        }

        private static void AppendCases(List<HarrisCriminalStyleDto> cases, List<HtmlNode> cells)
        {
            var dto = new HarrisCriminalStyleDto
            {
                Index = cases.Count + 1,
                CaseNumber = GetNumeric(cells[0].InnerText.Trim()),
                Style = GetStyle(cells[1]),
                FileDate = cells[2].InnerText.Trim(),
                Court = cells[3].InnerText.Trim(),
                Status = cells[4].InnerText.Trim(),
                TypeOfActionOrOffense = cells[5].InnerText.Trim()
            };
            cases.Add(dto);
        }

        private static string GetNumeric(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }
            return new string(text.TakeWhile(c => !Char.IsLetter(c)).ToArray());
        }

        private static string GetStyle(HtmlNode node)
        {
            if (node == null)
            {
                return string.Empty;
            }
            var anchor = node.SelectSingleNode("a");
            var strong = anchor.SelectSingleNode("strong");
            var text = strong.InnerText;
            return text.Split('(')[0].Trim();
        }
        private void ClickPageHyperlink(List<IWebElement> links, int pageId)
        {
            foreach (var item in links)
            {
                if (int.TryParse(item.Text.Trim(), out int index) && index == pageId)
                {
                    TheDriver.ClickAndOrSetText(item);
                    return;
                }
            }
        }
        private static int GetRecordCount(IWebElement pager, By by)
        {
            var element = pager.FindElement(by);
            var item = element.Text.Split(' ').ToList().Last();
            if (int.TryParse(item, out int rc))
            {
                return rc;
            }
            return 1;
        }

        private IWebElement GetElementOrFail(By by)
        {
            var isFound = TheDriver.IsElementPresent(by);
            if (isFound)
            {
                return TheDriver.FindElement(by);
            }
            throw new NoSuchElementException();
        }
        private List<IWebElement> GetElementsOrFail(By by)
        {
            var isFound = TheDriver.AreElementsPresent(by);
            if (isFound)
            {
                return TheDriver.FindElements(by).ToList();
            }
            throw new NoSuchElementException();
        }

        private class PopUpTable
        {
            public int Index { get; set; }
            public DateTime RunDate { get; set; }
            public List<HtmlNode> CauseSummary { get; set; }
            public List<HtmlNode> DefendantBio { get; set; }
            public List<HtmlNode> DefendantMetrics { get; set; }
            public List<HtmlNode> Court { get; set; }
            public List<HtmlNode> DefendantAddress { get; internal set; }

            public HarrisCriminalDto CriminalDto()
            {
                if (CauseSummary == null ||
                    DefendantBio == null ||
                    DefendantMetrics == null ||
                    Court == null)
                {
                    return null;
                }
                var dto = new HarrisCriminalDto
                {
                    Index = Index,
                    DateDatasetProduced = RunDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    CaseNumber = TryFindRow(CauseSummary, "Case"),
                    FilingDate = TryFindRow(CauseSummary, "File Date"),
                    CaseDisposition = TryFindRow(CauseSummary, "Case Disposition"),
                    Court = TryFindRow(Court, "Current Court"),
                    CaseStatus = TryFindRow(CauseSummary, "Case (Cause) Status"),
                    DefendantStatus = TryFindRow(CauseSummary, "Defendant Status"),
                    BondAmount = TryFindRow(CauseSummary, "Bond Amount"),
                    CurrentOffense = TryFindRow(CauseSummary, "Offense"),
                    CurrentOffenseLiteral = TryFindRow(CauseSummary, "Offense"),
                    DefendantRace = TryFindRow(DefendantBio, "Race/Sex"),
                    DefendantSex = TryFindRow(DefendantBio, "Race/Sex"),
                    DefendantDateOfBirth = TryFindRow(DefendantBio, "DOB"),
                    DefendantPlaceOfBirth = TryFindRow(DefendantMetrics, "Place Of Birth"),
                    DefUSCitizenFlag = TryFindRow(DefendantBio, "Place Of Birth")
                };
                var fullAddress = TryFindRow(DefendantAddress, "Address");
                UpdateAddress(dto, fullAddress);
                return dto;
            }

            private void UpdateAddress(HarrisCriminalDto dto, string fullAddress)
            {
                if (dto == null | string.IsNullOrEmpty(fullAddress))
                {
                    return;
                }
                var parsed = fullAddress.Split(' ');
                var zip = parsed.Last();
                // return
            }

            private string TryFindRow(List<HtmlNode> table, string heading)
            {
                var found = table.FirstOrDefault(f => 
                {
                    var cell = f.SelectNodes("td[@style='font-weight: bold']")?.ToList();
                    if (cell == null) return false;
                    var target = cell.FirstOrDefault(c => c.InnerText.Trim().Equals(heading, StringComparison.OrdinalIgnoreCase));
                    return target != null;
                });
                if (found == null) return string.Empty;
                var data = found.ParentNode.SelectNodes("//td")?.ToList().Last();
                if (data == null) return string.Empty;
                return data.InnerText.Trim();
            }
        }

    }
}
