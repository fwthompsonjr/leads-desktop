using LegalLead.PublicData.Search.Helpers;
using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        public static List<QueryDbResult> GetDataSource(this FileInfo fileInfo)
        {
            var filePath = fileInfo.FullName;
            if (!IsValidExcelPackage(filePath)) return null;
            var package = CreateExcelPackage(filePath);
            var worksheet = package.Workbook.Worksheets[0];
            var rows = worksheet.Dimension.Rows;
            var list = new List<QueryDbResult>();
            for (var r = 2; r < rows; r++)
            {
                list.Add(new()
                {
                    Name = worksheet.GetCellValue(r, GetIndex("Name")),
                    Zip = worksheet.GetCellValue(r, GetIndex("Zip")),
                    Address1 = worksheet.GetCellValue(r, GetIndex("Address1")),
                    Address2 = worksheet.GetCellValue(r, GetIndex("Address2")),
                    Address3 = worksheet.GetCellValue(r, GetIndex("Address3")),
                    CaseNumber = worksheet.GetCellValue(r, GetIndex("CaseNumber")),
                    DateFiled = worksheet.GetCellValue(r, GetIndex("Date Filed")),
                    Court = worksheet.GetCellValue(r, GetIndex("Court")),
                    CaseType = worksheet.GetCellValue(r, GetIndex("Case Type")),
                    CaseStyle = worksheet.GetCellValue(r, GetIndex("Case Style")),
                    Plaintiff = worksheet.GetCellValue(r, GetIndex("Plaintiff")),
                    County = worksheet.GetCellValue(r, GetIndex("County")),
                    CourtAddress = worksheet.GetCellValue(r, GetIndex("CourtAddress"))
                });
            }
            return list;
        }

        public static ExcelPackage CreateExcelPackage(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            try
            {
                using var stream = new MemoryStream(File.ReadAllBytes(filePath));
                return new ExcelPackage(stream);
            }
            catch
            {
                return null;
            }
        }
        public static bool IsValidExcelPackage(ExcelPackage package)
        {
            if (package == null || package.Workbook.Worksheets.Count == 0)
            {
                return false;
            }

            var worksheet = package.Workbook.Worksheets[0];

            if (worksheet.Dimension == null || worksheet.Dimension.Rows == 0 || worksheet.Dimension.Columns != expectedHeaders.Length)
            {
                return false;
            }
            var plaintiffId = GetPlaintiffColumnId();
            for (int col = 1; col <= expectedHeaders.Length; col++)
            {
                var columnId = col - 1;
                var cellValue = worksheet.GetValue(1, col);
                if (cellValue is not string cellText) return false;
                if (columnId == plaintiffId)
                {
                    if (!plaintiffNames.Contains(cellText)) return false;
                }
                else
                {
                    var expectedValue = expectedHeaders[columnId];
                    if (!expectedValue.Equals(cellText)) return false;
                }

            }

            return true;
        }
        public static bool IsValidExcelPackage(string filePath)
        {
            ExcelPackage package = CreateExcelPackage(filePath);
            if (package == null) return false;
            return IsValidExcelPackage(package);
        }

        private static string GetCellValue(this ExcelWorksheet worksheet, int row, int col)
        {
            var cellValue = worksheet.GetValue(row, col);
            if (cellValue is not string text) return string.Empty;
            return text;
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
        private static int GetIndex(string indexName)
        {
            if (string.IsNullOrEmpty(indexName)) return -1;
            if (plaintiffNames.Contains(indexName)) return GetPlaintiffColumnId() + 1;
            return expectedHeaders.ToList().FindIndex(x => x.Equals(indexName)) + 1;
        }
        private static int GetPlaintiffColumnId()
        {
            if (plaintiffColumnIndex.HasValue) return plaintiffColumnIndex.Value;
            var columnId = expectedHeaders.ToList().FindIndex(x => x == "Plaintiff");
            plaintiffColumnIndex = columnId;
            return columnId;
        }
        private static int? plaintiffColumnIndex;

        private static readonly string[] expectedHeaders = {
            "Name", "FirstName", "LastName", "Zip", "Address1", "Address2", "Address3",
            "CaseNumber", "Date Filed", "Court", "Case Type", "Case Style", "Plaintiff",
            "County", "CourtAddress"
        };
        private static readonly string[] plaintiffNames = { "Plantiff", "Plaintiff" };
    }
}
