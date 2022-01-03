using Harris.Criminal.Db.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thompson.RecordSearch.Utility.Web;

namespace Thompson.RecordSearch.Utility.Db
{
    [DataAction(Name = "header", ProcessId = 2)]
    public class GetDownloadAction : BaseAction
    {

        public GetDownloadAction(HccProcess process) : base(process)
        {
        }
        public override TimeSpan EstimatedDuration => TimeSpan.FromMinutes(3);

        public override async Task ExecuteAsync(IProgress<HccProcess> progress, HccProcess process)
        {
            ReportProgress = progress;
            Start();
            var fileName = await GetDownloadAsync().ConfigureAwait(false);
            Information($"File {fileName}. Downloaded");
            End();
            throw new NotImplementedException();
        }


        private async Task<string> GetDownloadAsync()
        {
            var downloaded = await Task.Run(() =>
            {
                var obj = new HarrisCriminalData();
                var result = obj.GetData(null);
                return result;
            }).ConfigureAwait(false);
            return downloaded;
        }
    }
}
