using OfficeOpenXml;
using Thompson.RecordSearch.Utility.Interfaces;

namespace Thompson.RecordSearch.Utility.Classes
{
    public class ExcelLayoutHarrisCivil : IExcelLayout
    {
        public virtual int WebsiteId => 30;
        public virtual void Configure(ExcelPackage package)
        {
            const string addresses = "Addresses";
            if (package == null) { return; }
            if (package.Workbook == null) { return; }
            if (package.Workbook.Worksheets[addresses] == null) { return; }
            var wk = package.Workbook.Worksheets[addresses];
            var cc = wk.Dimension.Columns;
            var rr = wk.Dimension.Rows + 1;
            var taddress = cc - 2;
            var tcourt = cc - 3;
            var saddress = cc;
            var scourt = cc - 1;
            var current = 1;
            while (current < rr)
            {
                wk.Cells[current, taddress].Value = wk.Cells[current, saddress].Value;
                wk.Cells[current, tcourt].Value = wk.Cells[current, scourt].Value;
                wk.Cells[current, saddress].Clear();
                wk.Cells[current, scourt].Clear();
                current++;
            }
        }
    }
}
