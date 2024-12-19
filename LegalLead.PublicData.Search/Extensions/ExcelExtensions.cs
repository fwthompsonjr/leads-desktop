using LegalLead.PublicData.Search.Helpers;
using LegalLead.PublicData.Search.Interfaces;
using OfficeOpenXml;

namespace LegalLead.PublicData.Search.Extensions
{
    internal static class ExcelExtensions
    {
        public static void SecureContent(this ExcelPackage package, string itemId)
        {
            if (IsAccountAdmin() || string.IsNullOrEmpty(itemId)) return;
            var wbk = package.Workbook;
            var worksheet = wbk.Worksheets[0];
            // hide all rows except header
            var rows = worksheet.Dimension.Rows;
            for (var i = rows; i > 1; i--)
            {
                var row = worksheet.Row(i);
                row.Hidden = true;
            }
            // protect worksheet with password
            worksheet.Protection.SetPassword(itemId);
            worksheet.Protection.IsProtected = true;
            // protect workbook with password
            wbk.Protection.SetPassword(itemId);

        }
        private static bool IsAccountAdmin()
        {
            return GetAccountIndexes().Equals("-1");
        }

        private static string GetAccountIndexes()
        {
            var container = SessionPersistenceContainer.GetContainer;
            var instance = container.GetInstance<ISessionPersistance>(ApiHelper.ApiMode);
            return instance.GetAccountPermissions();
        }
    }
}
