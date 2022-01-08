using Harris.Criminal.Db.Entities;
using System;
using System.Linq;

namespace Thompson.RecordSearch.Utility.Db
{
    public class DownloadDataProcess : BaseDataProcess
    {
        protected override HccProcess Execute(IProgress<HccProcess> progress, HccProcess process)
        {
            var actions = GetEnumerableOfType<BaseAction>("header", process).ToList();
            foreach (var item in actions)
            {
                if (actions.IndexOf(item) > 0)
                {
                    item.WebDriver = actions[0].WebDriver;
                }
                item.Execute(progress);
            }
            return process;
        }
    }
}
