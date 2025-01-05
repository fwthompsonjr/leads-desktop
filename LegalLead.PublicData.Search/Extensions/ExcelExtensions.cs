using LegalLead.PublicData.Search.Helpers;
using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Util;
using OfficeOpenXml;
using System;
using System.IO;
using System.Text;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search.Extensions
{
    public static class ExcelExtensions
    {
        public static void SecureContent(this ExcelPackage package, string itemId, string token = "")
        {
            var isAdmin = IsAccountAdmin();
            var wbk = package.Workbook;
            SetTrackingObject(wbk, itemId, isAdmin);
            if (isAdmin) return;
            var worksheet = wbk.Worksheets[0];
            // hide all rows except header
            var rows = worksheet.Dimension.Rows;
            for (var i = rows; i > 1; i--)
            {
                var row = worksheet.Row(i);
                row.Hidden = true;
            }
            worksheet.Cells[rows + 1, 1].Value = LockDataMessage;
            var pword = !string.IsNullOrEmpty(token) ? token : itemId;
            // protect worksheet with password
            worksheet.Protection.SetPassword(pword);
            worksheet.Protection.IsProtected = true;
        }

        public static bool UnlockContent(
            this string excelFileName, 
            IExcelLockingHelper helper = null,
            string jsonMetaData = "",
            bool isPaid = true)
        {
            helper ??= excelHelper;
            if (!helper.DoesFileExist(excelFileName)) return false;
            try
            {
                var package = helper.GetPackage(excelFileName);
                var wbk = package.Workbook;
                if (!helper.IsWorksheetProctected(package))
                {
                    // append tracking id if needed
                    var tracking = jsonMetaData.ToInstance<IndexDto>();
                    if (tracking == null) return true;
                    var addJs = GetTrackingObject(wbk, helper);
                    if (addJs != null) return !addJs.IsComplete;
                    var isTrackingAdded = SetTrackingObject(wbk, tracking.InvoiceId, isPaid);
                    if (!isTrackingAdded) return false;
                    return helper.Save(package);
                }
                if (!helper.UnlockWorksheet(package)) return false;
                var js = GetTrackingObject(wbk, helper);
                if (js == null || string.IsNullOrEmpty(js.Id) || js.IsComplete) return false;
                var remoteObj = helper.GetExcelDetail(js.Id);
                if (remoteObj is not GetExcelDetailResponse response) return false;
                if (!response.IsCompleted || string.IsNullOrEmpty(response.Password)) return false;
                return helper.Save(package);
            }
            catch
            {
                return false;
            }
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

        private static ExcelTrackingObject GetTrackingObject(
        ExcelWorkbook wbk,
        IExcelLockingHelper helper)
        {
            var prop = helper.GetPropertyValue(wbk, TrackingKeyName);
            if (prop is not string jsencoded) return default;
            var converted = Convert.FromBase64String(jsencoded);
            var js = Encoding.UTF8.GetString(converted).ToInstance<ExcelTrackingObject>();
            return js;
        }

        private static bool SetTrackingObject(ExcelWorkbook wbk, string itemId, bool isComplete)
        {
            try
            {
                var js =
                    Convert.ToBase64String(
                    Encoding.UTF8.GetBytes(new ExcelTrackingObject { Id = itemId, IsComplete = isComplete }.ToJsonString()));
                wbk.Properties.SetCustomPropertyValue(TrackingKeyName, js);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private sealed class ExcelTrackingObject
        {
            public string Id { get; set; }
            public bool IsComplete { get; set; }
        }


        private sealed class IndexDto
        {
            public string CustomerId { get; set; } = string.Empty;
            public string InvoiceId { get; set; } = string.Empty;
            public bool IsCorrected { get; set; }
        }
        private const string TrackingKeyName = "lead-tracking-index";
        private const string LockDataMessage = "Content is locked. Please complete invoice payment to view data.";
        private static readonly IExcelLockingHelper excelHelper
            = ActionSettingContainer.GetContainer.GetInstance<IExcelLockingHelper>();
    }
}
