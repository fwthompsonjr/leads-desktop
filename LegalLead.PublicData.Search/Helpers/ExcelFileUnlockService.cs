using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using System.Collections.Generic;
using System.Diagnostics;
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
            _collection.ForEach(c =>
            {
                if (ItemIsFoundAndCorrected(c))
                {
                    var payload = new
                    {
                        CustomerId = c.LeadUserId,
                        InvoiceId = c.Id,
                        c.ExcelName
                    };
                    _svcs.UpdateExcelStatus(payload.ToJsonString());
                }
            });
        }

        private static bool ItemIsFoundAndCorrected(UsageExcelIndex c)
        {
            Debug.WriteLine("Item {1} ( {0} ) is scheduled for correction.", c.Id, c.ExcelName);
            return true;
        }
    }
}
