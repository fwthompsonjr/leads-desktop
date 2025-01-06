using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Extensions;
using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Thompson.RecordSearch.Utility.Extensions;

namespace LegalLead.PublicData.Search.Helpers
{
    internal class ExcelFileUnlockService
    {
        private readonly IRemoteInvoiceHelper _svcs;
        private readonly List<UsageExcelIndex> _collection;
        public ExcelFileUnlockService(
            IRemoteInvoiceHelper helper,
            List<UsageExcelIndex> source)
        {
            _collection = source;
            _svcs = helper;
        }

        public void Process()
        {
            if (_collection == null || _collection.Count == 0) return;
            var dirPath = CommonFolderHelper.CommonFolder;
            if (!Directory.Exists(dirPath)) return;
            var fileNames = _collection.Select(c =>
            {
                var payload = new ItemCorrectionDto
                {
                    CustomerId = c.LeadUserId,
                    InvoiceId = c.Id,
                    ExcelName = c.ExcelName,
                    FullName = Path.Combine(dirPath, c.ExcelName) ?? string.Empty,
                    IsPaid = c.IsCompleted,
                };
                return payload;
            }).Distinct().ToList();
            // add a filter to exclude files where file-info-version matches a pattern
            fileNames.RemoveAll(x => !x.IsPaid);
            fileNames.RemoveAll(x => string.IsNullOrEmpty(x.ExcelName));
            fileNames.RemoveAll(x => !File.Exists(x.FullName));
            fileNames.ForEach(x =>
            {
                var json = x.ToJsonString();
                x.IsCorrected = x.FullName.UnlockContent(null, json);
            });

            var corrections = fileNames.FindAll(x =>
                x.IsCorrected &&
                !string.IsNullOrEmpty(x.InvoiceId) &&
                !string.IsNullOrEmpty(x.CustomerId));

            if (corrections.Count == 0) return;

            corrections.ForEach(c =>
            {
                var payload = new
                {
                    c.CustomerId,
                    c.InvoiceId,
                    c.ExcelName
                };
                _svcs.UpdateExcelStatus(payload.ToJsonString());
            });
        }

        private sealed class ItemCorrectionDto
        {
            public string CustomerId { get; set; }
            public string InvoiceId { get; set; }
            public string ExcelName { get; set; }
            public string FullName { get; set; }
            public bool IsCorrected { get; set; }
            public bool IsPaid { get; internal set; }
        }
    }
}
