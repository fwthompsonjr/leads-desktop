using Harris.Criminal.Db;
using Harris.Criminal.Db.Downloads;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Thompson.RecordSearch.Utility.Models;
using Thompson.RecordSearch.Utility.Parsing;

namespace Thompson.RecordSearch.Utility.Classes
{
    public class HarrisCriminalInteractive : TarrantWebInteractive
    {

        private static CultureInfo GetCulture => CultureInfo.InvariantCulture;
        private static StringComparison Oic => StringComparison.OrdinalIgnoreCase;

        public HarrisCriminalInteractive(WebNavigationParameter parameters) : base(parameters)
        {
        }

        public HarrisCriminalInteractive(WebNavigationParameter parameters,
            DateTime startDate, DateTime endingDate) : base(parameters, startDate, endingDate)
        {

        }

        public HarrisCriminalInteractive()
        {
        }

        public override WebFetchResult Fetch()
        {
            // settings have been retrieved from the constructor
            // get any output file to store data from extract
            var startingDate = GetParameterValue<DateTime>(CommonKeyIndexes.StartDate);
            var endingDate = GetParameterValue<DateTime>(CommonKeyIndexes.EndDate);

            var peopleList = new List<PersonAddress>();
            WebFetchResult webFetch = new WebFetchResult();
            var results = new SettingsManager().GetOutput(this);
            while (startingDate.CompareTo(endingDate) <= 0)
            {

                var cases = new List<HLinkDataRow>();
                var people = new List<PersonAddress>();
                List<HarrisCriminalDto> detailGroup = new List<HarrisCriminalDto>();
                if (CaseStyleDbInspector.HasHeader(startingDate))
                {
                    var db = Startup.CaseStyles.DataList;
                    var fileDate = startingDate.ToString("M/d/yyyy", GetCulture);
                    var found = db.Where(f => f.FileDate.Equals(fileDate, Oic)).ToList();
                    var group = found.GroupBy(x => x.CaseNumber).Select(y => y.First()).ToList();
                    cases.AddRange(group.ToHLinkData());
                }
                var caseNumbers = cases.Select(c => c.Case.Split('-')[0].Trim()).Distinct().ToList();
                if (CaseStyleDbInspector.HasDetail(startingDate))
                {
                    var db = Startup.Downloads.DataList;
                    var fileDate = startingDate.ToString("yyyyMMdd", GetCulture);
                    var details = new List<HarrisCriminalDto>();
                    foreach (var dataset in db)
                    {
                        var found = dataset.Data.Where(a => a.FilingDate.Equals(fileDate, Oic));
                        if (found == null) continue;
                        details.AddRange(found);
                    }
                    var group = details.GroupBy(x => x.CaseNumber).Select(y => y.First()).ToList();
                    detailGroup = group.FindAll(x => caseNumbers.Contains(x.CaseNumber));
                }

                foreach (var item in cases)
                {
                    var smallerList = detailGroup.FindAll(x => x.CaseNumber.Trim().Equals(item.Case.Split('-')[0].Trim(), StringComparison.OrdinalIgnoreCase));
                    people.AddRange(item.ToPersonAddress(smallerList));
                }

                peopleList.AddRange(people);
                peopleList.ForEach(p =>
                {
                    p = p.ToCalculatedNames();
                    p = p.ToCalculatedZip();
                });
                webFetch.PeopleList = peopleList;
                webFetch.CaseList = peopleList.ToHtml();
                startingDate = startingDate.AddDays(1);
            }
            webFetch.Result = results.FileName;
            return webFetch;
        }
    }
}
