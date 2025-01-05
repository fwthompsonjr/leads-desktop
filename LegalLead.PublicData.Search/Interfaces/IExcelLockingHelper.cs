using OfficeOpenXml;

namespace LegalLead.PublicData.Search.Interfaces
{
    public interface IExcelLockingHelper
    {
        bool DoesFileExist(string filePath);
        string GetPropertyValue(ExcelWorkbook wbk, string propertyName);
        ExcelPackage GetPackage(string filePath);
        object GetExcelDetail(string id);
        bool Save(ExcelPackage package);
        bool IsWorksheetProctected(ExcelPackage package);
        bool UnlockWorksheet(ExcelPackage package);
    }
}
