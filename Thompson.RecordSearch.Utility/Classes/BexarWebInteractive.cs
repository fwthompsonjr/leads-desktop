using System.Threading;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Classes
{
    public class BexarWebInteractive : BaseWebIneractive
    {
        public BexarWebInteractive(WebNavigationParameter parameters)
        {
            Parameters = parameters;
        }

        public override WebFetchResult Fetch(CancellationToken token)
        {
            return new WebFetchResult();
        }
    }
}