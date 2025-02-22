﻿using OpenQA.Selenium;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Interfaces
{
    public interface IElementActionBase
    {

        /// <value>
        /// The name of the action.
        /// </value>
        string ActionName { get; }

        /// <summary>
        /// Gets or sets the get assertion.
        /// </summary>
        /// <value>
        /// The get assertion.
        /// </value>
        ElementAssertion GetAssertion { get; set; }

        /// <summary>
        /// Gets or sets the get web.
        /// </summary>
        /// <value>
        /// The get web.
        /// </value>
        IWebDriver GetWeb { get; set; }

        /// <summary>
        /// Acts the specified step.
        /// </summary>
        /// <param name="navigationStep">The navigation step.</param>
        void Act(NavigationStep item);

        /// <summary>
        /// Gets or sets the outer HTML.
        /// </summary>
        /// <value>
        /// The outer HTML.
        /// </value>
        string OuterHtml { get; set; }

        WebNavigationParameter GetSettings(int index);
        IWebInteractive Interactive { get; set; }
    }
}
