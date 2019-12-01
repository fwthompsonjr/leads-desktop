namespace Thompson.RecordSearch.Utility.Interfaces
{
    public interface IExcelFileWriter
    {
        void SaveAs(OfficeOpenXml.ExcelPackage pck, string outputFileName);
    }
}
