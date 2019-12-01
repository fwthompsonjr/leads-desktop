using System;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Classes
{
    public interface IWebInteractive
    {
        /// <summary>
        /// Gets or sets the ending date.
        /// </summary>
        /// <value>
        /// The ending date.
        /// </value>
        DateTime EndingDate { get; set; }

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        WebNavigationParameter Parameters { get; set; }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        string Result { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>
        /// The start date.
        /// </value>
        DateTime StartDate { get; set; }

        /// <summary>
        /// Fetches this instance.
        /// </summary>
        /// <returns></returns>
        WebFetchResult Fetch();

        /// <summary>
        /// Reads from file.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        string ReadFromFile(string result);

        /// <summary>
        /// Removes the element.
        /// </summary>
        /// <param name="tableHtml">The table HTML.</param>
        /// <param name="tagName">Name of the tag.</param>
        /// <returns></returns>
        string RemoveElement(string tableHtml, string tagName);

        /// <summary>
        /// Removes the tag.
        /// </summary>
        /// <param name="tableHtml">The table HTML.</param>
        /// <param name="tagName">Name of the tag.</param>
        /// <returns></returns>
        string RemoveTag(string tableHtml, string tagName);
    }
}