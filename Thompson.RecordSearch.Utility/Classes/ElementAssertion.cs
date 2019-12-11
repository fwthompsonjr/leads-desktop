using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
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
            Console.WriteLine(CommonKeyIndexes.SearchingForElement, elementName);
            var found = PageDriver.FindElement(selector);
            Console.WriteLine(string.Format(CommonKeyIndexes.ElementNotFound, elementName));
            return true;
        }

        public void SetSelectedIndex(By selector, string elementName, int selectedIndex)
        {
            var cmmd = CommonKeyIndexes.SetSelectElementIndex;
            Console.WriteLine(cmmd, elementName, selectedIndex);
            var elementToClick = PageDriver.FindElement(selector);
            var id = elementToClick.GetAttribute(CommonKeyIndexes.IdLowerCase);
            var command = string.Format(CommonKeyIndexes.GetElementSetIndex,
                id, selectedIndex);
            var changecommand = string.Format(CommonKeyIndexes.ElementFireOnChange,
                id);
            var optionName = string.Format(CommonKeyIndexes.ElementGetOptionText,
                id, selectedIndex);

            var jse = (IJavaScriptExecutor)PageDriver;
            var rsp = jse.ExecuteScript(optionName);
            Console.WriteLine(CommonKeyIndexes.SetSelectOptionIndex, rsp.ToString());
            jse.ExecuteScript(command);
            jse.ExecuteScript(changecommand);
        }

        public void WaitForElementExist(By selector, string elementName, int secondsWait = 10)
        {
            Console.WriteLine(CommonKeyIndexes.WaitingForElement, elementName);
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
            var message = string.Format(CommonKeyIndexes.ElementTextNotFound, elementName, searchString);
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
            var message = string.Format(CommonKeyIndexes.ElementMatchTextNotFound, elementName, searchString, found.Text);
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
            var classes = found.GetAttribute(CommonKeyIndexes.ClassAttribute) ?? string.Empty;
            var allClasses = classes.Split(' ');
            var message = string.Format(CommonKeyIndexes.ElementClassNotFound, elementName, className);
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
            var classes = found.GetAttribute(CommonKeyIndexes.ClassAttribute) ?? string.Empty;
            var allClasses = classes.Split(' ');
            return allClasses.Any(x => x.Equals(className, StringComparison.CurrentCultureIgnoreCase));
        }

        public bool ContainsAttribute(By selector, string attributeName, string attributeValue)
        {
            var found = PageDriver.FindElement(selector, 10);
            var actual = found.GetAttribute(attributeName) ?? string.Empty;
            var message = string.Format(CommonKeyIndexes.ElementAttributeNotFound,
                found.GetAttribute(CommonKeyIndexes.IdLowerCase),
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
            var classes = found.GetAttribute(CommonKeyIndexes.ClassAttribute) ?? string.Empty;
            var allClasses = classes.Split(' ');
            var message = string.Format(CommonKeyIndexes.ClassNameFound, elementName, className);
            var hasClass = allClasses.Any(x => x.Equals(className, StringComparison.CurrentCultureIgnoreCase));
            if (hasClass)
            {
                Console.WriteLine(message);
            }
            return !hasClass;
        }

        public void Navigate(string target)
        {
            Console.WriteLine(CommonKeyIndexes.NavigateToUrlMessage, target);
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
                Console.WriteLine(CommonKeyIndexes.ClickingOnElement, controlId);
                var jse = (IJavaScriptExecutor)PageDriver;
                jse.ExecuteScript(string.Format(CommonKeyIndexes.ClickElementJs, controlId));
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
                Console.WriteLine(CommonKeyIndexes.SettingControlValue, controlId,
                    controlId.Equals(CommonKeyIndexes.Password, 
                        StringComparison.CurrentCultureIgnoreCase) ? 
                        CommonKeyIndexes.PasswordMask : controlValue);
                var jse = (IJavaScriptExecutor)PageDriver;
                jse.ExecuteScript(string.Format(CommonKeyIndexes.ControlSetValue, controlId, controlValue));
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
            var dteFmt = CommonKeyIndexes.DateTimeShort;
            var startDate = data.StartDate.ToString(dteFmt);
            var endDate = data.EndingDate.ToString(dteFmt);
            var searchTypeIndex = data.Parameters.Keys
                .FirstOrDefault(x => x.Name.Equals(CommonKeyIndexes.SearchComboIndex, 
                StringComparison.CurrentCultureIgnoreCase));
            var searchTypeId = searchTypeIndex == null ? 0 : Convert.ToInt32(searchTypeIndex.Value);
            var caseTypeIndex = data.Parameters.Keys
                .FirstOrDefault(x => x.Name.Equals(CommonKeyIndexes.CaseSearchType, 
                StringComparison.CurrentCultureIgnoreCase));
            var caseType = caseTypeIndex == null ? string.Empty : caseTypeIndex.Value;
            var districtSearchFlag = data.Parameters.Keys
                .FirstOrDefault(x => x.Name.Equals(CommonKeyIndexes.DistrictSearchType, 
                StringComparison.CurrentCultureIgnoreCase));
            var districtType = districtSearchFlag == null ? string.Empty : districtSearchFlag.Value;
            var isDistrictSearch = districtSearchFlag != null;
            var itms = data.Parameters.Instructions;
            if (!isDistrictSearch)
            {
                itms.RemoveAll(x => x.FriendlyName.StartsWith(CommonKeyIndexes.DistrictDash, StringComparison.CurrentCultureIgnoreCase));
            }
            else if(isDistrictSearch & itms.Count < 15)
            {
                itms = SettingsManager.GetInstructions(1);
            }

            // substitute parameters
            itms.ForEach(x => x.Value = x.Value.Replace(CommonKeyIndexes.StartDateQuery, startDate));
            itms.ForEach(x => x.Value = x.Value.Replace(CommonKeyIndexes.EndingDateQuery, endDate));
            itms.ForEach(x => x.Value = x.Value.Replace(CommonKeyIndexes.SetComboIndexQuery, searchTypeId.ToString()));
            if (!string.IsNullOrEmpty(caseType))
            {
                var crimalLink = data.Parameters.Keys
                    .FirstOrDefault(x => x.Name.Equals(CommonKeyIndexes.CriminalLinkQuery, 
                    StringComparison.CurrentCultureIgnoreCase));
                if(isCriminalSearch && crimalLink != null)
                {
                    caseType = crimalLink.Value;
                }
                var caseSearchItems = itms.FindAll(x => 
                    x.FriendlyName.Equals(CommonKeyIndexes.SearchHyperlink, 
                        StringComparison.CurrentCultureIgnoreCase));

                caseSearchItems.ForEach(x => x.Value = caseType);
            }
            if (!string.IsNullOrEmpty(districtType))
            {
                var districtItems = itms.FindAll(x => 
                    x.FriendlyName.Equals(CommonKeyIndexes.DistrictHyperlink, 
                        StringComparison.CurrentCultureIgnoreCase));
                districtItems.ForEach(x => x.Value = districtType);
            }
            const char comma = ',';
            var navigations = GetNavigationBases(startDate);
            // ?SetComboIndex
            foreach (var item in itms)
            {
                Console.WriteLine(
                    CommonKeyIndexes.WebNavInstructionMessage, 
                    item.Name, 
                    item.FriendlyName, 
                    item.Value);
                var navigator = navigations.FirstOrDefault(f =>
                    f.Name.Equals(item.Name, StringComparison.CurrentCultureIgnoreCase));
                if (navigator == null) continue;
                var webElement = navigator.Execute(item);
                if (webElement != null) return webElement;
                //switch (item.Name)
                //{
                //    case "WaitForNavigation":
                //        PageDriver.WaitForNavigation();
                //        break;
                //    case "WaitForElementExist":
                //        if (item.By == CommonKeyIndexes.IdProperCase)
                //        {
                //            WaitForElementExist(By.Id(item.Value), item.FriendlyName);
                //        }
                //        if (item.By == CommonKeyIndexes.XPath)
                //        {
                //            WaitForElementExist(By.XPath(item.Value), item.FriendlyName);
                //        }
                //        break;
                //    case "Click":
                //        if (item.By == CommonKeyIndexes.IdProperCase)
                //        {
                //            // WaitForElementExist(By.Id(item.Value), item.FriendlyName);
                //            PageDriver.FindElement(By.Id(item.Value)).Click();
                //        }
                //        if (item.By == CommonKeyIndexes.XPath)
                //        {
                //            PageDriver.FindElement(By.XPath(item.Value)).Click();
                //        }
                //        break;
                //    case "ClickElement":
                //        ClickElement(item.Value);
                //        break;
                //    case "SetControlValue":
                //        var indexes = item.Value.Split(comma);
                //        var idx = indexes[0];
                //        var txt = indexes[1];
                //        if (item.FriendlyName.Equals(CommonKeyIndexes.DateFiledOnTextBox, 
                //            StringComparison.CurrentCultureIgnoreCase))
                //        {
                //            txt = startDate;
                //        }
                //        ControlSetValue(idx, txt);
                //        break;
                //    case "GetElement":
                //        if (item.By == CommonKeyIndexes.IdProperCase)
                //        {
                //            // WaitForElementExist(By.Id(item.Value), item.FriendlyName);
                //            return PageDriver.FindElement(By.Id(item.Value));
                //        }
                //        if (item.By == CommonKeyIndexes.XPath)
                //        {
                //            return PageDriver.FindElement(By.XPath(item.Value));
                //        }
                //        break;
                //    case "SetComboIndex":
                //        var parms = item.Value.Split(comma);
                //        var parmId = parms[0];
                //        if(!int.TryParse(parms[1], out int parmIndex)){
                //            parmIndex = 0;
                //        }
                //        SetSelectedIndex(By.Id(parmId), item.FriendlyName, parmIndex);
                //        break;
                //    default:
                //        break;
                //}
            }
            return null;
        }


        protected List<ElementNavigationBase> GetNavigationBases(string startDate)
        {
            var list = ElementNavigations;
            list.ForEach(x => {
                x.StartDate = startDate;
                x.Assertion = this;
            });
            return list;
        }

        private static List<ElementNavigationBase> _navigationElements;

        private static List<ElementNavigationBase> ElementNavigations => 
            _navigationElements ?? (_navigationElements = GetNavigators());

        private static List<ElementNavigationBase> GetNavigators()
        {
            var type = typeof(ElementNavigationBase);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract)
                .ToList();
            var commands = new List<ElementNavigationBase>();
            types.ForEach(f => commands.Add((ElementNavigationBase)Activator.CreateInstance(f)));
            return commands;
        }

    }

}
