using LegalLead.PublicData.Search.Helpers;
using System;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search.Util
{
    internal class CollinUiInterative : BaseUiInteractive
    {
        public CollinUiInterative(WebNavigationParameter parameters,
            DateTime startDate,
            DateTime endingDate
            ) : base(parameters)
        {

            webmgr = WebFetchingProvider.GetInteractive(parameters, startDate, endingDate);

            if (webmgr is CollinWebInteractive web)
            {
                var credential = UsageGovernance.GetAccountCredential("collin");
                if (!string.IsNullOrEmpty(credential)) { web.Credential = credential; }
            }

        }
        private readonly IWebInteractive webmgr;
        public override WebFetchResult Fetch()
        {
            webmgr.DriverReadHeadless = DriverReadHeadless;
            var result = webmgr.Fetch();
            return result;
        }

        protected override string GetCourtAddress(string courtType, string court)
        {
            throw new NotImplementedException();
        }
        private static readonly SessionApiFilePersistence UsageGovernance
            = SessionPersistenceContainer
            .GetContainer
            .GetInstance<SessionApiFilePersistence>();
    }
}