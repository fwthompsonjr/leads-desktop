// GetCaseDetailAction
using Harris.Criminal.Db;
using Harris.Criminal.Db.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Web;

namespace Thompson.RecordSearch.Utility.Db
{
    [DataAction(Name = "detail", ProcessId = 2)]
    public class GetCaseDetailAction : BaseAction
    {

        public GetCaseDetailAction(HccProcess process) : base(process)
        {
        }
        public override TimeSpan EstimatedDuration => TimeSpan.FromMinutes(3);

        public override void Execute(IProgress<HccProcess> progress)
        {

            DateTime MxDate = DateTime.Now.AddDays(-1).Date;
            DateTime MnDate = MxDate.AddDays(GetOptionValue());
            
            ReportProgress = progress;
            Start();
            var fileName = GetDownload(MnDate, MxDate);
            Information($"File {fileName}. Downloaded");
            End();
        }
        
        private int GetOptionValue()
        {
            var data = GetOption();
            var list = data.Values.ToList();
			var listId = data.Index.GetValueOrDefault(0);
			var indexId = Convert.ToInt32(list[listId], CultureInfo.CurrentCulture);
            return -1 * indexId;
        }

        private static HccOptionDto GetOption()
        {

            var data = DataOptions.Read();
            var list = JsonConvert.DeserializeObject<List<HccOptionDto>>(data)
                .Where(a => a.Type.Equals("settings", StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault(b => b.Id.Equals(110));
            return list;
        }
        private List<Dto.HarrisCriminalStyleDto> GetDownload(DateTime mnDate, DateTime mxDate)
        {

            using (var obj = new HarrisCriminalCaseStyle())
            {
                var records = obj.GetCases(WebDriver, mnDate, mxDate);
                return records;
            }
        }
    }
}
