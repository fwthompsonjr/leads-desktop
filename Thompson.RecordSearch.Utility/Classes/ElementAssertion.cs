using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;

namespace Thompson.RecordSearch.Utility.Classes
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types",
        Justification = "Exception thrown from this method will stop automation.")]
    public class ElementAssertion
    {
        public ElementAssertion(IWebDriver driver)
        {
            PageDriver = driver;
        }

        public IWebDriver PageDriver { get; private set; }

        public bool DoesElementExist(By selector, string elementName)
        {
            Console.WriteLine("Searching for element: {0}", elementName);
            var found = PageDriver.FindElement(selector);
            Console.WriteLine(string.Format("Element {0}: was not found.", elementName));
            return true;
        }

        public void SetSelectedIndex(By selector, string elementName, int selectedIndex)
        {

            Console.WriteLine("Setting SELECT index for element: {0} to {1}", elementName, selectedIndex);
            var elementToClick = PageDriver.FindElement(selector);
            var id = elementToClick.GetAttribute("id");
            var command = string.Format("document.getElementById('{0}').selectedIndex={1};",
                id, selectedIndex);
            var changecommand = string.Format("document.getElementById('{0}').onchange();",
                id);
            var optionName = string.Format("var cbo = document.getElementById('{0}'); return cbo.options[{1}].text;",
                id, selectedIndex);

            var jse = (IJavaScriptExecutor)PageDriver;
            var rsp = jse.ExecuteScript(optionName);
            Console.WriteLine("Setting OPTION : {0} ", rsp.ToString());
            jse.ExecuteScript(command);
            jse.ExecuteScript(changecommand);
        }

        public void WaitForElementExist(By selector, string elementName, int secondsWait = 10)
        {
            Console.WriteLine("Waiting for element: {0}", elementName);
            try
            {
                var wait = new WebDriverWait(PageDriver, TimeSpan.FromSeconds(secondsWait));
#pragma warning disable 618
#pragma warning disable 436
                wait.Until(ExpectedConditions.ElementIsVisible(selector));
#pragma warning restore 436
#pragma warning restore 618
            }
            catch (Exception)
            {
                // not gonna fire errors
            }
        }

        public bool ContainsText(By selector, string elementName, string searchString)
        {
            if (!DoesElementExist(selector, elementName)) return false;
            var found = PageDriver.FindElement(selector, 10);
            var message = string.Format("Element {0}: expected text '{1}' not found.", elementName, searchString);
            if (!found.Text.Contains(searchString))
            {
                Console.WriteLine(message);
                return false;
            }
            
            return true;
        }

        public bool MatchText(By selector, string elementName, string searchString)
        {
            if (!DoesElementExist(selector, elementName)) return false;
            var found = PageDriver.FindElement(selector, 10);
            var message = string.Format("Element {0}: expected text '{1}' not matched to actual '{2}'.", elementName, searchString, found.Text);
            if (!found.Text.Equals(searchString, StringComparison.CurrentCulture))
            {
                Console.WriteLine(message);
                return false;
            }
            return true;
        }

        public bool ContainsClass(By selector, string elementName, string className)
        {
            if (!DoesElementExist(selector, elementName)) return false;
            var found = PageDriver.FindElement(selector, 10);
            var classes = found.GetAttribute("class") ?? string.Empty;
            var allClasses = classes.Split(' ');
            var message = string.Format("Element {0}: expected class '{1}' not found.", elementName, className);
            var hasClass = allClasses.Any(x => x.Equals(className, StringComparison.CurrentCultureIgnoreCase));
            if (!hasClass)
            {
                Console.WriteLine(message);
            }
            return hasClass;
        }

        public bool QueryByClass(By selector, string elementName, string className)
        {
            if (!DoesElementExist(selector, elementName)) return false;
            var found = PageDriver.FindElement(selector, 10);
            var classes = found.GetAttribute("class") ?? string.Empty;
            var allClasses = classes.Split(' ');
            return allClasses.Any(x => x.Equals(className, StringComparison.CurrentCultureIgnoreCase));
        }

        public bool ContainsAttribute(By selector, string attributeName, string attributeValue)
        {
            var found = PageDriver.FindElement(selector, 10);
            var actual = found.GetAttribute(attributeName) ?? string.Empty;
            var message = string.Format("Element {0}: expected attribute '{1}' not matched to expected '{2}', actual='{3}'.",
                found.GetAttribute("id"),
                attributeName,
                attributeValue,
                actual);
            if (string.IsNullOrEmpty(attributeValue) && string.IsNullOrEmpty(actual)) return true;
            var hasAttribute = actual.Equals(attributeValue, StringComparison.CurrentCultureIgnoreCase);
            if (!hasAttribute)
            {
                Console.WriteLine(message);
            }
            return hasAttribute;
        }

        public bool DoesNotContainsClass(By selector, string elementName, string className)
        {
            if (!DoesElementExist(selector, elementName)) return false;
            var found = PageDriver.FindElement(selector, 10);
            var classes = found.GetAttribute("class") ?? string.Empty;
            var allClasses = classes.Split(' ');
            var message = string.Format("Element {0}: expected class '{1}' is found.", elementName, className);
            var hasClass = allClasses.Any(x => x.Equals(className, StringComparison.CurrentCultureIgnoreCase));
            if (hasClass)
            {
                Console.WriteLine(message);
            }
            return !hasClass;
        }

        public void Navigate(string target)
        {
            Console.WriteLine("Navigate to URL: {0}", target);
            var newUri = new Uri(target);
            PageDriver.Navigate().GoToUrl(newUri);
        }




        /// <summary>
        /// Clicks the element.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="controlId">The control identifier.</param>
        public void ClickElement(string controlId)
        {
            try
            {
                Console.WriteLine(" ... Click element - {0} ", controlId);
                var jse = (IJavaScriptExecutor)PageDriver;
                jse.ExecuteScript(string.Format("document.getElementById('{0}').click();", controlId));
            }
            catch (Exception)
            {
                // no action as selenium is funny in this behavior
            }
        }




        /// <summary>
        /// Sets the text value.
        /// </summary>
        /// <param name="controlId">The control identifier.</param>
        /// <param name="controlValue">The control value.</param>
        public void ControlSetValue(string controlId, string controlValue)
        {
            try
            {
                if (controlId == null) controlId = string.Empty;
                if (controlValue == null) controlValue = string.Empty;
                Console.WriteLine(" ... Setting value - {0} := {1}", controlId,
                    controlId.Equals("Password", StringComparison.CurrentCultureIgnoreCase) ? "xxxxxxxxx" : controlValue);
                var jse = (IJavaScriptExecutor)PageDriver;
                jse.ExecuteScript(string.Format("document.getElementById('{0}').value='{1}';", controlId, controlValue));
            }
            catch (Exception)
            {
                // no action as selenium is funny in this behavior
            }
        }

        /// <summary>
        /// Populates web forms with appropriate data as determined by the settings file.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        internal IWebElement Process(WebInteractive data, bool isCriminalSearch = false)
        {
            const string dteFmt = "MM/dd/yyyy";
            var startDate = data.StartDate.ToString(dteFmt);
            var endDate = data.EndingDate.ToString(dteFmt);
            var searchTypeIndex = data.Parameters.Keys.FirstOrDefault(x => x.Name.Equals("SearchComboIndex", StringComparison.CurrentCultureIgnoreCase));
            var searchTypeId = searchTypeIndex == null ? 0 : Convert.ToInt32(searchTypeIndex.Value);
            var caseTypeIndex = data.Parameters.Keys.FirstOrDefault(x => x.Name.Equals("CaseSearchType", StringComparison.CurrentCultureIgnoreCase));
            var caseType = caseTypeIndex == null ? string.Empty : caseTypeIndex.Value;
            var districtSearchFlag = data.Parameters.Keys.FirstOrDefault(x => x.Name.Equals("DistrictSearchType", StringComparison.CurrentCultureIgnoreCase));
            var districtType = districtSearchFlag == null ? string.Empty : districtSearchFlag.Value;
            var isDistrictSearch = districtSearchFlag != null;
            var itms = data.Parameters.Instructions;
            if (!isDistrictSearch)
            {
                itms.RemoveAll(x => x.FriendlyName.StartsWith("District-", StringComparison.CurrentCultureIgnoreCase));
            }
            else if(isDistrictSearch & itms.Count < 15)
            {
                itms = SettingsManager.GetInstructions(1);
            }

            // substitute parameters
            itms.ForEach(x => x.Value = x.Value.Replace("?StartDate", startDate));
            itms.ForEach(x => x.Value = x.Value.Replace("?EndingDate", endDate));
            itms.ForEach(x => x.Value = x.Value.Replace("?SetComboIndex", searchTypeId.ToString()));
            if (!string.IsNullOrEmpty(caseType))
            {
                var crimalLink = data.Parameters.Keys.FirstOrDefault(x => x.Name.Equals("criminalLinkQuery", StringComparison.CurrentCultureIgnoreCase));
                if(isCriminalSearch && crimalLink != null)
                {
                    caseType = crimalLink.Value;
                }
                var caseSearchItems = itms.FindAll(x => x.FriendlyName.Equals("Search-Hyperlink", StringComparison.CurrentCultureIgnoreCase));
                caseSearchItems.ForEach(x => x.Value = caseType);
            }
            if (!string.IsNullOrEmpty(districtType))
            {
                var districtItems = itms.FindAll(x => x.FriendlyName.Equals("District-Hyperlink", StringComparison.CurrentCultureIgnoreCase));
                districtItems.ForEach(x => x.Value = districtType);
            }
            // ?SetComboIndex
            foreach (var item in itms)
            {
                Console.WriteLine("Name:= {0}, FriendlyName:= {1}, Value:= {2}", item.Name, item.FriendlyName, item.Value);
                switch (item.Name)
                {
                    case "WaitForNavigation":
                        PageDriver.WaitForNavigation();
                        break;
                    case "WaitForElementExist":
                        if (item.By == "Id")
                        {
                            WaitForElementExist(By.Id(item.Value), item.FriendlyName);
                        }
                        if (item.By == "XPath")
                        {
                            WaitForElementExist(By.XPath(item.Value), item.FriendlyName);
                        }
                        break;
                    case "Click":
                        if (item.By == "Id")
                        {
                            // WaitForElementExist(By.Id(item.Value), item.FriendlyName);
                            PageDriver.FindElement(By.Id(item.Value)).Click();
                        }
                        if (item.By == "XPath")
                        {
                            PageDriver.FindElement(By.XPath(item.Value)).Click();
                        }
                        break;
                    case "ClickElement":
                        ClickElement(item.Value);
                        break;
                    case "SetControlValue":
                        var indexes = item.Value.Split(',');
                        var idx = indexes[0];
                        var txt = indexes[1];
                        if (item.FriendlyName.Equals("Date-Filed-On-TextBox", StringComparison.CurrentCultureIgnoreCase))
                        {
                            txt = startDate;
                        }
                        ControlSetValue(idx, txt);
                        break;
                    case "GetElement":
                        if (item.By == "Id")
                        {
                            // WaitForElementExist(By.Id(item.Value), item.FriendlyName);
                            return PageDriver.FindElement(By.Id(item.Value));
                        }
                        if (item.By == "XPath")
                        {
                            return PageDriver.FindElement(By.XPath(item.Value));
                        }
                        break;
                    case "SetComboIndex":
                        var parms = item.Value.Split(',');
                        var parmId = parms[0];
                        if(!int.TryParse(parms[1], out int parmIndex)){
                            parmIndex = 0;
                        }
                        SetSelectedIndex(By.Id(parmId), item.FriendlyName, parmIndex);
                        break;
                    default:
                        break;
                }
            }
            return null;
        }
    }

}
