﻿using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Classes
{
    public static partial class WebUtilities
    {
        protected interface ICaseFetch
        {
            List<HLinkDataRow> GetListCaseDataRows();
        }

        protected class NonCriminalCaseFetch : ICaseFetch
        {
            protected WebInteractive Data { get; }
            public NonCriminalCaseFetch(WebInteractive data)
            {
                Data = data;
            }

            public virtual List<HLinkDataRow> GetListCaseDataRows()
            {
                if (DataRows == null)
                {
                    return new List<HLinkDataRow>();
                }
                var parameter = GetParameter(Data, CommonKeyIndexes.IsCriminalSearch);
                if (parameter != null)
                {
                    parameter.Value = CommonKeyIndexes.NumberZero;
                }
                var cases = Search(GetNavigationAddress(), DataRows);
                return cases;
            }

            protected List<HLinkDataRow> Search(string navTo, List<HLinkDataRow> cases)
            {
                using (var driver = GetWebDriver(Data.DriverReadHeadless))
                {
                    try
                    {
                        if (cases == null)
                        {
                            cases = new List<HLinkDataRow>();
                        }

                        var data = Data;
                        var helper = new ElementAssertion(driver);
                        _ = GetCaseData(data, ref cases, navTo, helper, driver);
                        GetPersonData(cases, driver, data);

                    }
                    catch
                    {
                        TryCloseDriver(driver);
                        throw;
                    }
                    finally
                    {
                        TryCloseDriver(driver);
                    }
                }

                return cases;
            }

            protected virtual void GetPersonData(List<HLinkDataRow> cases, IWebDriver driver, WebInteractive data)
            {
                if (cases == null)
                {
                    throw new ArgumentNullException(nameof(cases));
                }

                if (driver == null)
                {
                    throw new ArgumentNullException(nameof(driver));
                }

                if (data == null)
                {
                    throw new ArgumentNullException(nameof(data));
                }

                var people = cases.FindAll(x => !string.IsNullOrEmpty(x.WebAddress));
                var mx = people.Count;
                people.ForEach(d =>
                {
                    var indx = people.IndexOf(d) + 1;
                    data.EchoProgess(0, mx, indx, $"Fetching address details for item {indx} of {mx}.", true);
                    Find(driver, d);
                });
                data.CompleteProgess();
                _ = people.Count(p => !string.IsNullOrEmpty(p.Defendant));
            }

            protected List<HLinkDataRow> DataRows
            {
                get
                {
                    if (_dataRows != null)
                    {
                        return _dataRows;
                    }

                    var navTo = GetNavigationAddress();
                    if (string.IsNullOrEmpty(navTo))
                    {
                        return _dataRows;
                    }

                    _dataRows = new List<HLinkDataRow>();
                    return _dataRows;
                }
            }

            protected bool IncludeCriminalRecords()
            {
                var criminalCase = GetParameter(Data, CommonKeyIndexes.CriminalCaseInclusion);
                if (criminalCase == null)
                {
                    return false;
                }

                if (!int.TryParse(criminalCase.Value, out int index))
                {
                    return false;
                }

                return (index == 1);
            }

            protected void ModifyInstructions(string keyName)
            {
                var searchLink = CommonKeyIndexes.SearchHyperlink;
                const StringComparison ccic = StringComparison.CurrentCultureIgnoreCase;
                var key = GetParameter(Data, keyName);
                if (key == null)
                {
                    return;
                }

                var searchLinks =
                    Data.Parameters.Instructions
                    .FindAll(a => a.FriendlyName.Equals(searchLink, ccic));
                searchLinks.ForEach(s => s.Value = key.Value);
            }

            private WebNavigationKey GetBaseUri()
            {
                return GetParameter(Data, CommonKeyIndexes.BaseUri);
            }

            private WebNavigationKey GetQuery()
            {
                return GetParameter(Data, CommonKeyIndexes.Query);
            }

            protected string GetNavigationAddress()
            {
                var target = GetBaseUri();
                var query = GetQuery();
                if (target == null | query == null)
                {
                    return null;
                }
                return string.Format(
                    CultureInfo.CurrentCulture,
                    CommonKeyIndexes.QueryString, target.Value, query.Value);
            }

            private List<HLinkDataRow> _dataRows;

            private static void TryCloseDriver(IWebDriver driver)
            {
                try
                {
                    driver.Close();
                    driver.Quit();
                }
                catch (Exception)
                {
                    // take no action on fail
                }
            }
        }

        protected class CriminalCaseFetch : NonCriminalCaseFetch
        {
            // criminalCaseInclusion
            public CriminalCaseFetch(WebInteractive data) : base(data)
            {

            }

            public override List<HLinkDataRow> GetListCaseDataRows()
            {
                if (DataRows == null)
                {
                    return new List<HLinkDataRow>();
                }
                if (!IncludeCriminalRecords())
                {
                    return new List<HLinkDataRow>();
                }

                var parameter = GetParameter(Data, CommonKeyIndexes.IsCriminalSearch);
                if (parameter != null)
                {
                    parameter.Value = CommonKeyIndexes.NumberOne;
                }
                ModifyInstructions(CommonKeyIndexes.CriminalLinkQuery);
                var cases = Search(GetNavigationAddress(), DataRows);
                cases.ForEach(c => c.IsCriminal = true);
                return cases;
            }


        }


        internal static WebNavigationKey GetParameter(
            WebInteractive data,
            string parameterName)
        {
            if (data == null)
            {
                return null;
            }

            if (data.Parameters == null)
            {
                return null;
            }

            if (data.Parameters.Keys == null)
            {
                return null;
            }

            if (!data.Parameters.Keys.Any())
            {
                return null;
            }

            const StringComparison ccic = StringComparison.CurrentCultureIgnoreCase;
            var keys = data.Parameters.Keys;
            return keys.Find(k => k.Name.Equals(parameterName, ccic));

        }



    }
}
