using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Classes
{
    public abstract class BaseWebIneractive : IWebInteractive
    {


        #region Properties
        protected const StringComparison comparisonIngore = StringComparison.CurrentCultureIgnoreCase;
        /// <summary>
        /// Gets or sets the parameters used to interact with public website.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        public WebNavigationParameter Parameters { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>
        /// The start date.
        /// </value>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the ending date.
        /// </summary>
        /// <value>
        /// The ending date.
        /// </value>
        public DateTime EndingDate { get; set; }

        /// <summary>
        /// Gets or sets the filename of the local results file.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public string Result { get; set; }

        #endregion

        #region Public Properties


        /// <summary>
        /// Removes the tag.
        /// </summary>
        /// <param name="tableHtml">The table HTML.</param>
        /// <param name="tagName">Name of the tag.</param>
        /// <returns></returns>
        public string RemoveTag(string tableHtml, string tagName)
        {
            if (string.IsNullOrEmpty(tableHtml))
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(tagName))
            {
                return string.Empty;
            }

            var provider = CultureInfo.CurrentCulture;
            var openTg = string.Format(provider, CommonKeyIndexes.OpenHtmlTag, tagName);
            var closeTg = string.Format(provider, CommonKeyIndexes.CloseHtmlTag, tagName);
            var idx = tableHtml.IndexOf(openTg, comparisonIngore);
            if (idx < 0)
            {
                return tableHtml;
            }

            var eidx = tableHtml.IndexOf(closeTg, comparisonIngore);
            var firstHalf = tableHtml.Substring(0, idx);
            var secHalf = tableHtml.Substring(eidx + closeTg.Length);
            return string.Concat(firstHalf, secHalf);
        }


        /// <summary>
        /// Removes the an HTML element from the DOM.
        /// Used to get rid of non-xml compliant img tags
        /// which can can error when reading table data from xml reader
        /// </summary>
        /// <param name="tableHtml">The table HTML.</param>
        /// <param name="tagName">Name of the tag.</param>
        /// <returns></returns>
        public string RemoveElement(string tableHtml, string tagName)
        {
            if (string.IsNullOrEmpty(tableHtml))
            {
                return tableHtml;
            }

            if (string.IsNullOrEmpty(tagName))
            {
                return tableHtml;
            }

            if (!tableHtml.Contains(tagName))
            {
                return tableHtml;
            }

            var idx = tableHtml.IndexOf(tagName, comparisonIngore);
            while (idx > 0)
            {
                var firstPart = tableHtml.Substring(0, idx);
                var lastPart = tableHtml.Substring(idx);
                var cidx = lastPart.IndexOf(CommonKeyIndexes.ImageCloseTag, comparisonIngore);
                tableHtml = string.Concat(firstPart, lastPart.Substring(cidx));
                idx = tableHtml.IndexOf(tagName, comparisonIngore);
            }
            return tableHtml;
        }


        /// <summary>
        /// Reads from file to get case data elements.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>An HTML table containing case data results</returns>
        public string ReadFromFile(string result)
        {
            if (!File.Exists(result))
            {
                return string.Empty;
            };
            var contents = File.ReadAllText(result);
            if (contents.Contains(CommonKeyIndexes.ImageOpenTag))
            {
                contents = RemoveElement(contents, CommonKeyIndexes.ImageOpenTag);
            }
            var doc = XmlDocProvider.GetDoc(contents);
            var ndeCase = doc.DocumentElement.SelectSingleNode(CommonKeyIndexes.CaseDataXpath);
            if (ndeCase == null)
            {
                return string.Empty;
            }

            if (!ndeCase.HasChildNodes)
            {
                return string.Empty;
            }

            return ((XmlCDataSection)ndeCase.ChildNodes[0]).Data;
        }

        /// <summary>
        /// Performs web scraping activities to fetches data 
        /// from web source.
        /// </summary>
        /// <returns></returns>
        public abstract WebFetchResult Fetch();

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the parameter value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyName">Name of the key.</param>
        /// <returns></returns>
        internal T GetParameterValue<T>(string keyName)
        {
            if (Parameters == null)
            {
                return default;
            }

            if (Parameters.Keys == null)
            {
                return default;
            }

            var item = Parameters.Keys.First(k => k.Name.Equals(keyName, comparisonIngore));
            if (item == null)
            {
                return default;
            }

            var obj = Convert.ChangeType(item.Value, typeof(T), CultureInfo.CurrentCulture);
            return (T)obj;
        }

        /// <summary>
        /// Sets the parameter value.
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <param name="keyValue">The key value.</param>
        protected void SetParameterValue(string keyName, string keyValue)
        {

            if (Parameters == null)
            {
                return;
            }

            if (Parameters.Keys == null)
            {
                return;
            }

            var item = Parameters.Keys.First(k => k.Name.Equals(keyName, comparisonIngore));
            if (item == null)
            {
                return;
            }

            item.Value = keyValue;
        }

        #endregion
        #region Static Methods

        /// <summary>
        /// Gets the web navigation.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="startingDate">The starting date.</param>
        /// <param name="endingDate">The ending date.</param>
        /// <returns></returns>
        public static WebNavigationParameter
            GetWebNavigation(int id,
            DateTime startingDate,
            DateTime endingDate)
        {

            var settings = SettingsManager
                .GetNavigation().Find(x => x.Id == id);
            var datelist = new List<string> { CommonKeyIndexes.StartDate, CommonKeyIndexes.EndDate };
            var keys = settings.Keys.FindAll(s => datelist.Contains(s.Name));
            keys.First().Value = startingDate.ToString(CommonKeyIndexes.DateTimeShort, CultureInfo.CurrentCulture.DateTimeFormat);
            keys.Last().Value = endingDate.ToString(CommonKeyIndexes.DateTimeShort, CultureInfo.CurrentCulture.DateTimeFormat);
            return settings;
        }

        #endregion
    }
}
