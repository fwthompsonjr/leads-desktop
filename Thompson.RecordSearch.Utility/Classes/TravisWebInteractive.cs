using System.Threading;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Classes
{
    public class TravisWebInteractive : BaseWebIneractive
    {
        public TravisWebInteractive(WebNavigationParameter parameters)
        {
            Parameters = parameters;
        }

        public override WebFetchResult Fetch(CancellationToken token)
        {
            return new WebFetchResult();
        }
    }
}