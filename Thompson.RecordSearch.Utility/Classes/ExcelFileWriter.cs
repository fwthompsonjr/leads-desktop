using System.IO;
using OfficeOpenXml;
using Thompson.RecordSearch.Utility.Interfaces;

namespace Thompson.RecordSearch.Utility.Classes
{
    public class ExcelFileWriter : IExcelFileWriter
    {

        public void SaveAs(ExcelPackage pck, string outputFileName)
        {
            FileInfo fileInfo = new FileInfo(outputFileName);
            pck.SaveAs(fileInfo);
        }
    }
}
