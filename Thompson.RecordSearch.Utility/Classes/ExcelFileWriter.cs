using System.IO;
using OfficeOpenXml;
using Thompson.RecordSearch.Utility.Interfaces;

namespace Thompson.RecordSearch.Utility.Classes
{
    public class ExcelFileWriter : IExcelFileWriter
    {

        public void SaveAs(ExcelPackage pck, string outputFileName)
        {
            if (pck == null) throw new System.ArgumentNullException(nameof(pck));
            if(string.IsNullOrEmpty(outputFileName)) throw new System.ArgumentNullException(nameof(outputFileName));
            FileInfo fileInfo = new FileInfo(outputFileName);
            pck.SaveAs(fileInfo);
        }
    }
}
