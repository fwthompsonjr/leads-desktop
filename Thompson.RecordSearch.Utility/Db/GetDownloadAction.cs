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

        public override void Execute(IProgress<HccProcess> progress)
        {
            ReportProgress = progress;
            Start();
            var fileName = GetDownload();
            Information($"File {fileName}. Downloaded");
            End();
        }

        private string GetDownload()
        {
            using (var obj = new HarrisCriminalData())
            {
                var result = obj.GetData(WebDriver);
                return result;
            }
        }
    }
}
