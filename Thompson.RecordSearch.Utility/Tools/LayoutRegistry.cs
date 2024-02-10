using StructureMap;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Interfaces;

namespace Thompson.RecordSearch.Utility.Tools
{
    public class LayoutRegistry : Registry
    {
        public LayoutRegistry()
        {
            For<IExcelLayout>().Use<ExcelLayoutHarrisCivil>();
            For<IExcelLayout>().Add<ExcelLayoutHarrisCivil>().Named("1");
            For<IExcelLayout>().Add<ExcelLayoutHarrisCivil>().Named("10");
            For<IExcelLayout>().Add<ExcelLayoutHarrisCivil>().Named("20");
            For<IExcelLayout>().Add<ExcelLayoutHarrisCivil>().Named("30");
        }
    }
}
