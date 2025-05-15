using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using Thompson.RecordSearch.Utility.Dto;

namespace LegalLead.PublicData.Search.Util
{
    public class HarrisFetchPersonDetail : BaseHarrisSearchAction
    {
        public override int OrderId => 75;
        internal int ExpectedRecords { get; set; }
        public override object Execute()
        {
            if (Parameters == null || Driver == null)
                throw new NullReferenceException(ERR_DRIVER_UNAVAILABLE);
            var find = By.XPath("//a[@class = 'doclinks']");            
            const int pageOne = 1;
            var alldata = new List<CaseItemDto>();
            var iterations = pageOne;
            bool hasNextPage = false;
            while (iterations == pageOne || hasNextPage)
            {
                WaitForExists(Driver, find, true);
                var found = ReadCaseItems(Driver);
                alldata.AddRange(found); 
                iterations++;
                hasNextPage = HasNextPage(Driver);
                if (!hasNextPage) break;
                Console.WriteLine($"Fetching page ({iterations}) of case detail");
                GoToNextPage(Driver);
            }
            return JsonConvert.SerializeObject(alldata);
        }
    }
}