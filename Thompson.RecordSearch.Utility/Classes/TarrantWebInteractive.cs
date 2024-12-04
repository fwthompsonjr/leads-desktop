using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Thompson.RecordSearch.Utility.Addressing;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Interfaces;
using Thompson.RecordSearch.Utility.Models;
using Thompson.RecordSearch.Utility.Web;

namespace Thompson.RecordSearch.Utility.Classes
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
    public partial class TarrantWebInteractive : WebInteractive
    {
        protected const StringComparison comparison = StringComparison.CurrentCultureIgnoreCase;
        #region Constructors
        public TarrantWebInteractive() { }

        public TarrantWebInteractive(WebNavigationParameter parameters) : base(parameters)
        {

        }
        public TarrantWebInteractive(WebNavigationParameter parameters, DateTime startDate, DateTime endingDate) : base(parameters, startDate, endingDate)
        {
            var formatDate = CultureInfo.CurrentCulture.DateTimeFormat;
            SetParameterValue(CommonKeyIndexes.StartDate,
                startDate.ToString(CommonKeyIndexes.DateTimeShort, formatDate));
            SetParameterValue(CommonKeyIndexes.EndDate,
                endingDate.ToString(CommonKeyIndexes.DateTimeShort, formatDate));
        }

        #endregion

        #region Properties


        #endregion

        public override WebFetchResult Fetch()
        {
            // settings have been retrieved from the constructor
            // get any output file to store data from extract
            var startingDate = GetParameterValue<DateTime>(CommonKeyIndexes.StartDate);
            var endingDate = GetParameterValue<DateTime>(CommonKeyIndexes.EndDate);
            var customSearch = GetParameterValue<int>(CommonKeyIndexes.CriminalCaseInclusion); // "criminalCaseInclusion");
            var peopleList = new List<PersonAddress>();
            WebFetchResult webFetch = null;
            var fetchers = (new FetchProvider(this)).GetFetches(customSearch);
            var formatDate = CultureInfo.CurrentCulture.DateTimeFormat;
            while (startingDate.CompareTo(endingDate) <= 0)
            {
                Console.WriteLine($"Date {startingDate:d}. Reading data.");
                SetParameterValue(CommonKeyIndexes.StartDate,
                    startingDate.ToString(CommonKeyIndexes.DateTimeShort, formatDate));
                SetParameterValue(CommonKeyIndexes.EndDate,
                    startingDate.ToString(CommonKeyIndexes.DateTimeShort, formatDate));
                foreach (var obj in fetchers)
                {

                    obj.Fetch(startingDate, out webFetch, out List<PersonAddress> people);
                    peopleList.AddRange(people);
                    webFetch.PeopleList = peopleList;
                }
                startingDate = startingDate.AddDays(1);
            }
            return webFetch;
        }


        private WebFetchResult SearchWeb(XmlContentHolder results,
            List<NavigationStep> steps,
            DateTime startingDate,
            DateTime endingDate,
            ref List<HLinkDataRow> cases,
            out List<PersonAddress> people)
        {
            IWebDriver driver = WebUtilities.GetWebDriver(DriverReadHeadless);

            try
            {
                return Search(results, steps, startingDate, endingDate, ref cases, out people, driver);

            }
            catch (Exception)
            {
                driver.Quit();
                driver.Dispose();
                throw;
            }
            finally
            {
                driver.Quit();
            }
        }

        private WebFetchResult SearchWeb(
            int customSearchType,
            XmlContentHolder results,
            List<NavigationStep> steps,
            DateTime startingDate,
            DateTime endingDate,
            ref List<HLinkDataRow> cases,
            out List<PersonAddress> people)
        {
            IWebDriver driver = WebUtilities.GetWebDriver();

            try
            {
                var fetched = Search(results, steps, startingDate, endingDate, ref cases, out people, driver);
                if (customSearchType != 2)
                {
                    return fetched;
                }

                var caseList = cases.ToList();
                people = fetched.PeopleList;
                people.ForEach(p =>
                {
                    var source = caseList.Find(c => c.Case.Equals(p.CaseNumber, StringComparison.CurrentCultureIgnoreCase));
                    if (source == null)
                    {
                        return;
                    }

                    if (string.IsNullOrEmpty(source.PageHtml))
                    {
                        return;
                    }

                    var dto = DataPointLocatorDto.Load(source.PageHtml);
                    p.CaseStyle = dto.DataPoints
                        .First(f =>
                            f.Name.Equals(CommonKeyIndexes.CaseStyle,
                            StringComparison.CurrentCultureIgnoreCase)).Result;
                });
                // people = ExtractPeople(cases);



                return new WebFetchResult
                {
                    Result = results.FileName,
                    CaseList = fetched.CaseList,
                    PeopleList = people
                };

            }
            catch (Exception)
            {
                driver.Quit();
                driver.Dispose();
                throw;
            }
            finally
            {
                driver.Quit();
                driver.Dispose();
            }
        }

        private WebFetchResult Search(XmlContentHolder results,
            List<NavigationStep> steps, DateTime startingDate,
            DateTime endingDate,
            ref List<HLinkDataRow> cases,
            out List<PersonAddress> people,
            IWebDriver driver)
        {
            var assertion = new ElementAssertion(driver);
            var caseList = string.Empty;
            var formatDate = CultureInfo.CurrentCulture.DateTimeFormat;
            ElementActions.ForEach(x => x.GetAssertion = assertion);
            ElementActions.ForEach(x => x.GetWeb = driver);
            ElementActions.ForEach(x => x.Interactive = this);
            foreach (var item in steps)
            {
                // if item action-name = 'set-text'
                var actionName = item.ActionName;
                if (item.ActionName.Equals(CommonKeyIndexes.SetText,
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    if (item.DisplayName.Equals(CommonKeyIndexes.StartDate, //"startDate", 
                        StringComparison.CurrentCultureIgnoreCase))
                    {
                        item.ExpectedValue =
                            startingDate.Date.ToString(CommonKeyIndexes.DateTimeShort, formatDate);
                    }

                    if (item.DisplayName.Equals(CommonKeyIndexes.EndDate,
                        StringComparison.CurrentCultureIgnoreCase))
                    {
                        item.ExpectedValue =
                            endingDate.Date.ToString(CommonKeyIndexes.DateTimeShort, formatDate);
                    }
                }
                var action = ElementActions
                    .Find(x =>
                    x.ActionName.Equals(item.ActionName,
                    StringComparison.CurrentCultureIgnoreCase));
                if (action == null) continue;
                action.Act(item);
                cases = ExtractCaseData(results, cases, actionName, action);
                if (string.IsNullOrEmpty(caseList) && !string.IsNullOrEmpty(action.OuterHtml))
                {
                    caseList = action.OuterHtml;
                }
            }
            var subset = cases.FindAll(c => string.IsNullOrEmpty(c.Address));
            var mx = subset.Count;
            subset.ForEach(c =>
            {
                var indx = subset.IndexOf(c) + 1;
                this.EchoProgess(0, mx, indx, $"Reading address details {indx} of {mx}", true);
                GetAddressInformation(driver, this, c);
            });
            this.CompleteProgess();
            people = ExtractPeople(cases);



            return new WebFetchResult
            {
                Result = results.FileName,
                CaseList = caseList,
                PeopleList = people
            };
        }

        protected virtual List<PersonAddress> ExtractPeople(List<HLinkDataRow> cases)
        {
            if (cases == null)
            {
                return null;
            }

            if (!cases.Any())
            {
                return new List<PersonAddress>();
            }

            var list = new List<PersonAddress>();
            foreach (var item in cases)
            {
                var styleInfo = GetCaseStyle(item);
                var person = new PersonAddress
                {
                    Name = item.Defendant,
                    CaseNumber = item.Case,
                    DateFiled = item.DateFiled,
                    Court = item.Court,
                    CaseType = item.CaseType,
                    CaseStyle = styleInfo
                };
                person = ParseAddress(item.Address, person);
                list.Add(person);
            }
            return list;
        }


        /// <summary>
        /// Extracts the case data.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <param name="cases">The cases.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        protected virtual List<HLinkDataRow> ExtractCaseData(XmlContentHolder results,
            List<HLinkDataRow> cases,
            string actionName, IElementActionBase action)
        {
            if (results == null)
            {
                throw new ArgumentNullException(nameof(results));
            }

            if (cases == null)
            {
                throw new ArgumentNullException(nameof(cases));
            }

            if (actionName == null)
            {
                throw new ArgumentNullException(nameof(actionName));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (!actionName.Equals("get-table-html", comparison))
            {
                return cases;
            }

            if (string.IsNullOrEmpty(action.OuterHtml))
            {
                return cases;
            }

            var htmlAction = (ElementGetHtmlAction)action;
            var isProbate = htmlAction.IsProbateSearch;
            var isJustice = htmlAction.IsJusticeSearch;

            // create a list of hlinkdatarows from table
            var caseData = RemoveElement(action.OuterHtml, "<img");
            // remove colspan? <colgroup>
            caseData = RemoveTag(caseData, "colgroup");
            // load cases into [cases] object

            var newcases = LoadFromHtml(caseData);
            newcases.FindAll(x => !x.IsProbate).ForEach(c => c.IsProbate = isProbate);
            newcases.FindAll(x => !x.IsJustice).ForEach(c => c.IsJustice = isJustice);
            // map case information using file xpath
            newcases = AppendCourtInformation(newcases);

            // add this to the result file
            AppendToResult(results.FileName, caseData, "results/result[@name='casedata']");
            cases.AddRange(newcases);
            return cases;
        }
        protected string GetCaseStyle(HLinkDataRow item)
        {
            if (item == null)
            {
                return string.Empty;
            }

            var doc = XmlDocProvider.GetDoc(item.Data);
            if (!doc.FirstChild.HasChildNodes)
            {
                return string.Empty;
            }

            if (doc.FirstChild.ChildNodes.Count < 6)
            {
                return string.Empty;
            }

            var colIndex = Parameters.Id == 10 ? 2 : 2;
            var node = doc.FirstChild.ChildNodes[colIndex];
            if (node == null)
            {
                return string.Empty;
            }

            return node.InnerText;
        }
        protected static PersonAddress ParseAddress(string address, PersonAddress person)
        {
            var separator = @"<br/>";
            var pipe = '|';
            var pipeString = "|";
            const string noMatch = "No Match Found|Not Matched 00000";
            if (person == null)
            {
                throw new ArgumentNullException(nameof(person));
            }

            if (string.IsNullOrEmpty(address)) { address = noMatch; }
            address = new StringBuilder(address.Trim()).Replace(separator, pipeString).ToString();
            if (address.EndsWith(pipeString, StringComparison.CurrentCultureIgnoreCase))
            {
                address = address.Substring(0, address.Length - 1);
            }
            var pieces = address.Split(pipe)
                .ToList().FindAll(s => !string.IsNullOrEmpty(s));
            if (!pieces.Any())
            {
                return person;
            }
            person.Address1 = pieces.First().Trim();
            person.Address3 = pieces.Last().Trim();
            if (pieces.Count > 2)
            {
                var mx = pieces.Count - 1;
                person.Address2 = string.Empty;
                for (int i = 1; i < mx; i++)
                {
                    person.Address2 = string.Format(
                        CultureInfo.CurrentCulture,
                        "{0} {1}",
                        person.Address2, pieces[i].Trim()).Trim();
                }
            }
            var zipPart = person.Address3.Split(' ').ToList();
            person.Zip = zipPart.Last();

            return person;
        }

        private static void GetAddressInformation(IWebDriver driver, TarrantWebInteractive jsonWebInteractive, HLinkDataRow linkData)
        {
            var fmt = jsonWebInteractive.GetParameterValue<string>(CommonKeyIndexes.HlinkUri);//"hlinkUri");
            var xpath = jsonWebInteractive.GetParameterValue<string>(CommonKeyIndexes.PersonNodeXpath); // "personNodeXpath");
            var helper = new ElementAssertion(driver);
            helper.Navigate(string.Format(CultureInfo.CurrentCulture,
                fmt, linkData.WebAddress));
            driver.WaitForNavigation();
            var tdName = TryFindElement(driver, By.XPath(xpath));
            if (tdName == null)
            {
                return;
            }

            linkData.Defendant = tdName.GetAttribute(CommonKeyIndexes.InnerText);
            var parent = tdName.FindElement(By.XPath(CommonKeyIndexes.ParentElement));
            linkData.Address = parent.Text;
            try
            {
                // check or set IsCriminal attribute of linkData object
                var criminalLink = TryFindElement(driver, By.XPath(CommonKeyIndexes.CriminalLinkXpath));
                if (criminalLink != null) { linkData.IsCriminal = true; }
                // get row index of this element ... and then go one row beyond...
                var ridx = parent.GetAttribute(CommonKeyIndexes.RowIndex);
                var table = parent.FindElement(By.XPath(CommonKeyIndexes.ParentElement));
                var trCol = table.FindElements(By.TagName(CommonKeyIndexes.TrElement));
                if (!int.TryParse(ridx, out int r))
                {
                    return;
                }

                parent = GetAddressRow(parent, trCol); // put this row-index into config... it can change
                linkData.Address = new StringBuilder(parent.Text)
                    .Replace(Environment.NewLine, "<br/>").ToString();
                var findCase = new FindCaseDataPoint();
                findCase.Find(driver, linkData);
                driver.Navigate().Back();
            }
            catch (Exception)
            {

            }
        }


        private static void AppendToResult(string fileName, string caseData, string xpath)
        {
            var doc = XmlDocProvider.Load(fileName);
            var ndeCase = doc.DocumentElement.SelectSingleNode(xpath);
            if (ndeCase == null)
            {
                return;
            }

            if (!ndeCase.HasChildNodes)
            {
                return;
            } ((XmlCDataSection)ndeCase.ChildNodes[0]).Data = caseData;
            doc.Save(fileName);
        }

        private static List<HLinkDataRow> LoadFromHtml(string caseData)
        {
            var caseList = new List<HLinkDataRow>();
            XmlDocument doc = XmlDocProvider.GetDoc(caseData);
            var trElements = doc.FirstChild.ChildNodes[0].SelectNodes("tr").Cast<XmlNode>().ToList();
            foreach (var trow in trElements)
            {
                var link = trow.SelectSingleNode("td/a");
                if (link == null)
                {
                    continue;
                }

                var href = link.Attributes.GetNamedItem("href");
                if (href == null)
                {
                    continue;
                }

                caseList.Add(new HLinkDataRow
                {
                    WebAddress = href.InnerText,
                    Data = trow.OuterXml
                });
            }

            return caseList;
        }

        private List<HLinkDataRow> AppendCourtInformation(List<HLinkDataRow> caseList
            )
        {
            var parameterId = Parameters.Id;
            var contents = SettingsManager.Content;
            var doc = XmlDocProvider.GetDoc(contents);
            List<XmlNode> caseInspetor = GetCaseInspector(parameterId, doc);
            var probateInspector = GetCaseInspector(parameterId, doc, "probate");
            var justiceInspector = GetCaseInspector(parameterId, doc, "justice");
            if (probateInspector == null) probateInspector = caseInspetor;
            if (justiceInspector == null) justiceInspector = caseInspetor;
            //var caseInfo = doc.DocumentElement.SelectSingleNode("");
            foreach (var item in caseList)
            {
                var data = item.Data;
                var dcc = XmlDocProvider.GetDoc(data);
                var trow = dcc.ChildNodes[0];
                var inspector = item.IsProbate ? probateInspector : caseInspetor;
                if (item.IsJustice) { inspector = justiceInspector; }
                if (!MapCourtAttributes(item, trow, inspector))
                {
                    MapCourtAttributes(item, trow, probateInspector);
                    item.IsProbate = true;
                    item.CriminalCaseStyle = item.CaseStyle;
                }
            }
            return caseList;
        }

        private static bool MapCourtAttributes(HLinkDataRow item, XmlNode trow, List<XmlNode> inspector)
        {
            foreach (var search in inspector)
            {
                var node = trow.SelectSingleNode(search.InnerText);
                var keyName = search.Attributes.GetNamedItem("name").InnerText;
                item[keyName] = string.Empty;
                if (node == null)
                {
                    Debug.WriteLine("Unable to locate element {0} with selector - {1}",
                        search.Attributes.GetNamedItem("name").InnerText,
                        search.InnerText);
                    return false;
                }
                item[keyName] = node.InnerText;
            }
            return true;
        }

        private static List<XmlNode> GetCaseInspector(int parameterId, XmlDocument doc, string typeName = "normal")
        {
            try
            {

                var directions = doc.DocumentElement.SelectSingleNode("directions");
                if (directions == null) return null;
                var inspection = directions.SelectNodes("caseInspection");
                if (inspection == null) return null;
                var indexes = inspection.Cast<XmlNode>().ToList()
                    .FindAll(x =>
                    {
                        var attr = x.Attributes.GetNamedItem("id");
                        if (attr == null) return false;
                        return attr.Value == parameterId.ToString(CultureInfo.CurrentCulture.NumberFormat);
                    });
                if (indexes == null || indexes.Count == 0) return null;
                var inspector = indexes.Find(x => x.Attributes.GetNamedItem("type").Value == typeName);
                if (inspector == null) return null;
                return inspector.ChildNodes.Cast<XmlNode>().ToList();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Locates the address element from the case-detail drill down page.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="trCol">The tr col.</param>
        /// <returns></returns>
        private static IWebElement GetAddressRow(IWebElement parent,
            System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> trCol)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            int colIndex = 3;
            parent = trCol[colIndex];
            var txt = string.IsNullOrEmpty(parent.Text) ? "" : parent.Text.Trim();
            if (string.IsNullOrEmpty(txt))
            {
                parent = trCol[colIndex - 1];
            }

            return parent;
        }


        /// <summary>Tries the find element on a specfic web page using the By condition supplied.</summary>
        /// <param name="parent">The parent web browser instance.</param>
        /// <param name="by">The by condition used to locate the element</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design",
            "CA1031:Do not catch general exception types",
            Justification = "Returning a NULL allows the caller to handle")]
        internal static IWebElement TryFindElement(IWebDriver parent, By by)
        {
            try
            {
                return parent.FindElement(by);
            }
            catch (Exception)
            {
                return null;
            }
        }

        #region Element Action Helpers

        private static List<IElementActionBase> elementActions;

        protected static List<IElementActionBase> ElementActions
        {
            get { return elementActions ?? (elementActions = GetActions()); }
        }

        protected static List<IElementActionBase> GetActions()
        {
            var container =
            ActionElementContainer.GetContainer;
            return container.GetAllInstances<IElementActionBase>().ToList();
        }

        protected static NavigationInstructionDto GetAppSteps(string suffix = "")
        {
            const string dataFormat = @"{0}\xml\{1}.json";
            var appDirectory = ContextManagment.AppDirectory;
            var dataFile = string.Format(
                CultureInfo.CurrentCulture,
                dataFormat,
                appDirectory,
                suffix);
            if (!File.Exists(dataFile))
            {
                throw new FileNotFoundException(
                    CommonKeyIndexes.NavigationFileNotFound);
            }
            var data = File.ReadAllText(dataFile);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<NavigationInstructionDto>(data);

        }

        private static TarrantCourtDropDownDto _tarrantComboBxValue;

        protected static TarrantCourtDropDownDto TarrantComboBxValue => _tarrantComboBxValue ?? (_tarrantComboBxValue = GetComboBoxValues());
        public static TarrantCourtDropDownDto GetComboBoxValues()
        {
            const string dataFormat = @"{0}\xml\{1}.json";
            const string suffix = "tarrantCourtSearchDropDown";
            var appDirectory = ContextManagment.AppDirectory;
            var dataFile = string.Format(
                CultureInfo.CurrentCulture,
                dataFormat,
                appDirectory,
                suffix);
            if (!File.Exists(dataFile))
            {
                throw new FileNotFoundException(
                    CommonKeyIndexes.NavigationFileNotFound);
            }
            var data = File.ReadAllText(dataFile);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<TarrantCourtDropDownDto>(data);

        }
        #endregion
    }
}
