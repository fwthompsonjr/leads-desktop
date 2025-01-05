using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Util;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegalLead.PublicData.Search.Helpers
{
    public class ExcelLockingHelper : IExcelLockingHelper
    {
        public bool DoesFileExist(string filePath)
        {
            return File.Exists(filePath);
        }

        public ExcelPackage GetPackage(string filePath) {
            return new ExcelPackage(new FileInfo(filePath));
        }

        public object GetExcelDetail(string id)
        {
            return dbHelper.GetExcelDetail(id);
        }

        public string GetPropertyValue(ExcelWorkbook wbk, string propertyName)
        {
            var obj = wbk.Properties.GetCustomPropertyValue(propertyName);
            if (obj is not string encoded) return string.Empty;
            return encoded;
        }

        public bool IsWorksheetProctected(ExcelPackage package)
        {
            try
            {
                var wbk = package.Workbook;
                var worksheet = wbk.Worksheets[0];
                var protection = worksheet.Protection;
                return protection.IsProtected;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UnlockWorksheet(ExcelPackage package)
        {
            try 
            {
                if (!IsWorksheetProctected(package)) return false;
                var wbk = package.Workbook;
                var worksheet = wbk.Worksheets[0];
                var protection = worksheet.Protection;
                protection.SetPassword("");
                protection.IsProtected = false;
                // unhide all rows
                var rows = worksheet.Dimension.Rows;
                for (var i = rows; i > 1; i--)
                {
                    var row = worksheet.Row(i);
                    row.Hidden = false;
                    var cell = worksheet.Cells[i, 1];
                    if (cell.Value is string txt && txt.Equals(LockDataMessage))
                    {
                        cell.Value = string.Empty;
                    }
                }
                return true;
            } 
            catch 
            { 
                return false;
            }
        }

        public bool Save(ExcelPackage package)
        {
            try
            {
                package.Save();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private const string LockDataMessage = "Content is locked. Please complete invoice payment to view data.";

        private static readonly IRemoteDbHelper dbHelper
            = ActionSettingContainer.GetContainer.GetInstance<IRemoteDbHelper>();
    }
}
