using OfficeOpenXml;

namespace Thompson.RecordSearch.Utility.Interfaces
{
    public interface IExcelLayout
    {

        int WebsiteId { get; }
        void Configure(ExcelPackage package);
    }
}
