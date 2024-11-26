using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

            void Fetch(DateTime startingDate, out WebFetchResult webFetch, out List<PersonAddress> people, int? caseOverrideId = null);
        }

        class NonCriminalFetch : ITarrantWebFetch
        {
            public NonCriminalFetch(TarrantWebInteractive tarrantWeb)
            {
                Web = tarrantWeb;
            }

            protected static class FetchType
            {
                public const int NonCriminal = 0;
                public const int Criminal = 2;
            }

            protected const StringComparison Ccic = StringComparison.CurrentCultureIgnoreCase;
            public virtual string Name => "NonCriminal";

            public virtual TarrantWebInteractive Web { get; }

            public virtual void Fetch(DateTime startingDate, out WebFetchResult webFetch, out List<PersonAddress> people, int? caseOverrideId = null)
            {
                var steps = new List<NavigationStep>();
                var navigationFile = Web.GetParameterValue<string>(CommonKeyIndexes.NavigationControlFile);
                var sources = navigationFile.Split(',').ToList();
                if (caseOverrideId == null)
                {
                    caseOverrideId = TarrantComboBxValue.CourtMap.First(x => x.Name.Equals("Justice of Peace", Ccic)).Id;
                }
                sources.ForEach(s => steps.AddRange(GetAppSteps(s).Steps));
                SetupParameters(steps, caseOverrideId, out people, out XmlContentHolder results, out List<HLinkDataRow> cases);
                webFetch = Web.SearchWeb(results, steps, startingDate, startingDate, ref cases, out people);
            }



            protected void SetupParameters(List<NavigationStep> steps,
                int? caseTypeOverrideId,
                out List<PersonAddress> people,
                out XmlContentHolder results,
                out List<HLinkDataRow> cases)
            {
                results = new SettingsManager().GetOutput(Web);
                cases = new List<HLinkDataRow>();
                people = new List<PersonAddress>();

                var caseTypeId = caseTypeOverrideId ?? Web.GetParameterValue<int>(CommonKeyIndexes.CaseTypeSelectedIndex);
                // set special item values
                var caseTypeSelect = steps.First(x => x.ActionName.Equals(CommonKeyIndexes.SetSelectValue, StringComparison.CurrentCultureIgnoreCase));
                caseTypeSelect.ExpectedValue = caseTypeId.ToString(CultureInfo.CurrentCulture);
            }
        }

        class NonCrimalFetchProbateCourt : NonCriminalFetch
        {
            public NonCrimalFetchProbateCourt(TarrantWebInteractive tarrantWeb) : base(tarrantWeb) { }
            public override string Name => "NonCriminalProbateCount";

            public override void Fetch(DateTime startingDate, out WebFetchResult webFetch, out List<PersonAddress> people, int? caseOverrideId = null)
            {
                var overrideId = TarrantComboBxValue.CourtMap.First(x => x.Name.Equals("Probate", Ccic)).Id;
                base.Fetch(startingDate, out webFetch, out people, overrideId);
            }
        }
        class NonCrimalFetchCclCourt : NonCriminalFetch
        {
            public NonCrimalFetchCclCourt(TarrantWebInteractive tarrantWeb) : base(tarrantWeb) { }
            public override string Name => "NonCriminalCclCount";

            public override void Fetch(DateTime startingDate, out WebFetchResult webFetch, out List<PersonAddress> people, int? caseOverrideId = null)
            {
                var overrideId = TarrantComboBxValue.CourtMap.First(x => x.Name.Equals("Court Court at Law", Ccic)).Id;
                base.Fetch(startingDate, out webFetch, out people, overrideId);
            }
        }

        class CriminalFetch : NonCriminalFetch
        {
            public CriminalFetch(TarrantWebInteractive tarrantWeb)
                : base(tarrantWeb)
            {
            }
            public override string Name => "Criminal";
            public override void Fetch(DateTime startingDate, out WebFetchResult webFetch, out List<PersonAddress> people, int? caseOverrideId = null)
            {
                var steps = new List<NavigationStep>();
                var navigationFile = Web.GetParameterValue<string>("navigation.control.alternate.file");
                var sources = navigationFile.Split(',').ToList();
                sources.ForEach(s => steps.AddRange(GetAppSteps(s).Steps));
                SetupParameters(steps, null, out people, out XmlContentHolder results, out List<HLinkDataRow> cases);
                webFetch = Web.SearchWeb(FetchType.Criminal, results, steps, startingDate, startingDate, ref cases, out people);
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
                const string criminal = "criminal";
                const StringComparison ccic = StringComparison.CurrentCultureIgnoreCase;
                var fetchers = new List<ITarrantWebFetch>
                {
                    new NonCriminalFetch(Web),
                    new NonCrimalFetchProbateCourt(Web),
                    new NonCrimalFetchCclCourt(Web),
                    new CriminalFetch(Web)
                };
                switch (searchMode)
                {
                    case 0:
                        fetchers = fetchers.FindAll(x =>
                        {
                            var lowered = x.Name.ToLower(CultureInfo.CurrentCulture);
                            return lowered.StartsWith(criminal, ccic);
                        });
                        break;
                    case 2:
                        fetchers = fetchers.FindAll(x =>
                        {
                            var lowered = x.Name.ToLower(CultureInfo.CurrentCulture);
                            return !lowered.StartsWith(criminal, ccic);
                        });
                        break;
                    default:
                        break;
                }


                return fetchers;
            }
        }


    }
}
