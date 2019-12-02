using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Classes
{
    public partial class TarrantWebInteractive
    {
        interface ITarrantWebFetch
        {
            TarrantWebInteractive TarrantWeb { get; }

            void Fetch(DateTime startingDate, out WebFetchResult webFetch, out List<PersonAddress> people);
        }

        class TarrantWebFetch : ITarrantWebFetch
        {
            public TarrantWebFetch(TarrantWebInteractive tarrantWeb)
            {
                TarrantWeb = tarrantWeb;
            }

            public TarrantWebInteractive TarrantWeb { get; }

            public virtual void Fetch(DateTime startingDate, out WebFetchResult webFetch, out List<PersonAddress> people)
            {
                var results = new SettingsManager().GetOutput(TarrantWeb);
                // need to open the navigation file(s)
                var steps = new List<Step>();
                var navigationFile = TarrantWeb.GetParameterValue<string>("navigation.control.file");
                var sources = navigationFile.Split(',').ToList();
                var cases = new List<HLinkDataRow>();
                people = new List<PersonAddress>();
                sources.ForEach(s => steps.AddRange(TarrantWeb.GetAppSteps(s).Steps));

                var caseTypeId = TarrantWeb.GetParameterValue<int>("caseTypeSelectedIndex");
                // set special item values
                var caseTypeSelect = steps.First(x => x.ActionName.Equals("set-select-value"));
                caseTypeSelect.ExpectedValue = caseTypeId.ToString();
                webFetch = TarrantWeb.SearchWeb(results, steps, startingDate, startingDate, ref cases, out people);
            }
        }


        class TarrantWebCriminalFetch : TarrantWebFetch
        {
            public TarrantWebCriminalFetch(TarrantWebInteractive tarrantWeb) 
                : base(tarrantWeb)
            {
            }

            public override void Fetch(DateTime startingDate, out WebFetchResult webFetch, out List<PersonAddress> people)
            {
                var results = new SettingsManager().GetOutput(TarrantWeb);
                // need to open the navigation file(s)
                var steps = new List<Step>();
                const string navigationFile = @"tarrantCountyMapping_2";
                var sources = navigationFile.Split(',').ToList();
                var cases = new List<HLinkDataRow>();
                people = new List<PersonAddress>();
                sources.ForEach(s => steps.AddRange(TarrantWeb.GetAppSteps(s).Steps));

                var caseTypeId = TarrantWeb.GetParameterValue<int>("caseTypeSelectedIndex");
                // set special item values
                var caseTypeSelect = steps.First(x => x.ActionName.Equals("set-select-value"));
                caseTypeSelect.ExpectedValue = caseTypeId.ToString();
                webFetch = TarrantWeb.SearchWeb(results, steps, startingDate, startingDate, ref cases, out people);
            }
        }


        class TarrantFetchProvider
        {

            public TarrantWebInteractive TarrantWeb { get; }
            public TarrantFetchProvider(TarrantWebInteractive tarrantWeb)
            {
                TarrantWeb = tarrantWeb;
            }
            public List<ITarrantWebFetch> GetFetches()
            {
                var provider = new VersionNameProvider();
                var isFuture = provider.Name.Equals(provider.VersionNames.Last());
                var fetchers = new List<ITarrantWebFetch> 
                { 
                    new TarrantWebFetch(TarrantWeb)
                };
                if (!isFuture) return fetchers;
                fetchers.Add(new TarrantWebCriminalFetch(TarrantWeb));
                return fetchers;
            }
        }
    }
}
