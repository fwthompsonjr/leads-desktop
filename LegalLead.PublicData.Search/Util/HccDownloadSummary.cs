using System;
using Thompson.RecordSearch.Utility.Dto;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class HccDownloadSummary : HccDownloadMonthly
    {
        public override int OrderId => 20;
        public override object Execute()
        {
            if (!IsDownloadRequested) { return true; }
            var model = HccConfigurationModel.GetModel();
            var js = FindRecordJs(model.Settings);
            var executor = GetJavaScriptExecutor();
            DownloadShortName = $"{model.Settings}.txt";
            if (Parameters == null || Driver == null || executor == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);
            return RequestDownload(js, executor);
        }
    }
}
