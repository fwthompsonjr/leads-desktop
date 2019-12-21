using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Thompson.RecordSearch.Utility.Addressing;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Classes
{
    public partial class TarrantWebInteractive
    {
        interface ITarrantWebFetch
        {
            string Name { get; }
            TarrantWebInteractive Web { get; }

            void Fetch(DateTime startingDate, out WebFetchResult webFetch, out List<PersonAddress> people);
        }

        class NonCriminalFetch : ITarrantWebFetch
        {
            public NonCriminalFetch(TarrantWebInteractive tarrantWeb)
            {
                Web = tarrantWeb;
            }
            public virtual string Name => "NonCriminal";

            public TarrantWebInteractive Web { get; }

            public virtual void Fetch(DateTime startingDate, out WebFetchResult webFetch, out List<PersonAddress> people)
            {
                var steps = new List<NavigationStep>();
                var navigationFile = Web.GetParameterValue<string>(CommonKeyIndexes.NavigationControlFile); // "navigation.control.file");
                var sources = navigationFile.Split(',').ToList();
                sources.ForEach(s => steps.AddRange(GetAppSteps(s).Steps));
                SetupParameters(steps, out people, out XmlContentHolder results, out List<HLinkDataRow> cases);
                webFetch = Web.SearchWeb(results, steps, startingDate, startingDate, ref cases, out people);
            }

            protected void SetupParameters(List<NavigationStep> steps, out List<PersonAddress> people, out XmlContentHolder results, out List<HLinkDataRow> cases)
            {
                results = new SettingsManager().GetOutput(Web);
                cases = new List<HLinkDataRow>();
                people = new List<PersonAddress>();

                var caseTypeId = Web.GetParameterValue<int>(CommonKeyIndexes.CaseTypeSelectedIndex); // "caseTypeSelectedIndex");
                // set special item values
                var caseTypeSelect = steps.First(x => x.ActionName.Equals(CommonKeyIndexes.SetSelectValue, StringComparison.CurrentCultureIgnoreCase));
                caseTypeSelect.ExpectedValue = caseTypeId.ToString(CultureInfo.CurrentCulture);
            }
        }


        class CriminalFetch : NonCriminalFetch
        {
            public CriminalFetch(TarrantWebInteractive tarrantWeb)
                : base(tarrantWeb)
            {
            }
            public override string Name => "Criminal";
            public override void Fetch(DateTime startingDate, out WebFetchResult webFetch, out List<PersonAddress> people)
            {
                var steps = new List<NavigationStep>();
                var navigationFile = Web.GetParameterValue<string>("navigation.control.alternate.file");
                var sources = navigationFile.Split(',').ToList();
                sources.ForEach(s => steps.AddRange(GetAppSteps(s).Steps));
                SetupParameters(steps, out people, out XmlContentHolder results, out List<HLinkDataRow> cases);
                webFetch = Web.SearchWeb(2, results, steps, startingDate, startingDate, ref cases, out people);
            }
        }


        class FetchProvider
        {

            public TarrantWebInteractive Web { get; }
            public FetchProvider(TarrantWebInteractive tarrantWeb)
            {
                Web = tarrantWeb;
            }
            public List<ITarrantWebFetch> GetFetches(int searchMode = 2)
            {
                var fetchers = new List<ITarrantWebFetch>
                {
                    new NonCriminalFetch(Web),
                    new CriminalFetch(Web)
                };
                switch (searchMode)
                {
                    case 0:
                        fetchers.RemoveAt(1);
                        break;
                    case 2:
                        fetchers.RemoveAt(0);
                        break;
                    default:
                        break;
                }


                return fetchers;
            }
        }


    }
}
