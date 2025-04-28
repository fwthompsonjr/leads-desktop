using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Helpers;
using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using LegalLead.PublicData.Search.Util;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Models;
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

        public static string GenerateExcelFileName(this List<PersonAddress> people,
            GenExcelFileParameter context,
            bool isTest = false)
        {
            var websiteId = context.WebsiteId;
            string countyName = context.CountyName;
            string courtType = context.CourtType;
            string trackingIndex = context.TrackingIndex;
            DateTime startDate = context.StartDate;
            DateTime endDate = context.EndDate;
            var folder = GetExcelDirectoryName;
            var name = DallasSearchProcess.GetCourtName(courtType);
            var fmt = $"{countyName}_{name}_{GetDateString(startDate)}_{GetDateString(endDate)}";
            var fullName = GetUniqueFileName(folder, fmt, Path.Combine(folder, $"{fmt}.xlsx"));
            var writer = new ExcelWriter();
            var content = writer.ConvertToPersonTable(addressList: people, worksheetName: "addresses", websiteId: websiteId);
            var courtlist = people.Select(p =>
            {
                if (string.IsNullOrEmpty(p.Court)) return string.Empty;
                var find = GetCourtAddress(websiteId, name, p.Court);
                if (string.IsNullOrEmpty(find)) return string.Empty;
                return find;
            }).ToList();
            content.TransferColumn("County", "fname");
            content.TransferColumn("CourtAddress", "lname");
            content.PopulateColumn("CourtAddress", courtlist);
            content.SecureContent(trackingIndex);
            using (var ms = new MemoryStream())
            {
                content.SaveAs(ms);
                var data = ms.ToArray();
                if (!isTest) File.WriteAllBytes(fullName, data);
            }
            return fullName;

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
        private static string excelDirectoyName = null;

        private static readonly string[] expectedHeaders = {
            "Name", "FirstName", "LastName", "Zip", "Address1", "Address2", "Address3",
            "CaseNumber", "Date Filed", "Court", "Case Type", "Case Style", "Plaintiff",
            "County", "CourtAddress"
        };
        private static readonly string[] plaintiffNames = { "Plantiff", "Plaintiff" };
        private static string GetExcelDirectoryName => excelDirectoyName ??= ExcelDirectoyName();
        private static string ExcelDirectoyName()
        {
            var appFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var xmlFolder = Path.Combine(appFolder, "data");
            if (!Directory.Exists(xmlFolder)) Directory.CreateDirectory(xmlFolder);
            return xmlFolder;
        }

        private static string GetDateString(DateTime date)
        {
            const string fmt = "MMddyy";
            return date.ToString(fmt, culture);
        }

        private static string GetUniqueFileName(string folder, string fmt, string fullName)
        {
            int idx = 1;
            while (File.Exists(fullName))
            {
                fullName = Path.Combine(folder, $"{fmt}_{idx:D4}.xlsx");
                idx++;
            }
            return fullName;
        }

        private static string GetCourtAddress(int websiteId, string courtType, string courtName)
        {
            var address = websiteId switch
            {
                1 => AlternateCourtLookupService.GetAddress(websiteId, courtName),
                10 => AlternateCourtLookupService.GetAddress(websiteId, courtName),
                20 => AlternateCourtLookupService.GetAddress(websiteId, courtName),
                30 => AlternateCourtLookupService.GetAddress(websiteId, courtName),
                40 => HccCourtLookupService.GetAddress(courtName),
                60 => DallasCourtLookupService.GetAddress(courtType, courtName),
                70 => TravisCourtLookupService.GetAddress(courtType, courtName),
                80 => BexarCourtLookupService.GetAddress(courtType, courtName),
                90 => HidalgoCourtLookupService.GetAddress(courtType, courtName),
                100 => ElPasoCourtLookupService.GetAddress(courtType, courtName),
                110 => FortBendCourtLookupService.GetAddress(courtType, courtName),
                120 => WilliamsonCourtLookupService.GetAddress(courtType, courtName),
                130 => GraysonCourtLookupService.GetAddress(courtType, courtName),
                _ => AlternateCourtLookupService.GetAddress(websiteId, courtName),
            };
            return address;
        }
        private static readonly CultureInfo culture = new CultureInfo("en-US");
    }
}
