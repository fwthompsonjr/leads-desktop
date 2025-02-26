using LegalLead.PublicData.Search.Models;
using System.Collections.Generic;
using System.IO;
using Thompson.RecordSearch.Utility;

namespace LegalLead.PublicData.Search.Classes
{
    public class SearchResult
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public string Website { get; set; }

        public string ResultFileName { get; set; }

        public string Search { get; set; }

        public string SearchDate { get; set; }

        public bool IsCompleted { get; set; }
        public int Id { get; internal set; }
        public List<QueryDbResponse> AddressList { get; set; } = [];
        public void MoveToCommon()
        {
            if (string.IsNullOrEmpty(ResultFileName)) return;
            var tmpFileName = ResultFileName.Replace(CommonKeyIndexes.ExtensionXml, CommonKeyIndexes.ExtensionXlsx);
            var movedFile = CommonFolderHelper.MoveToCommon(tmpFileName);
            if (string.IsNullOrEmpty(movedFile) || !File.Exists(movedFile)) return;
            ResultFileName = movedFile;
        }
    }
}
