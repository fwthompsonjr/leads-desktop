using System.Threading;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Classes
{
    public class DallasWebInteractive : BaseWebIneractive
    {
        public DallasWebInteractive(WebNavigationParameter parameters)
        {
            Parameters = parameters;
        }

        public override WebFetchResult Fetch(CancellationToken token)
        {
            return new WebFetchResult();
        }
    }
}
