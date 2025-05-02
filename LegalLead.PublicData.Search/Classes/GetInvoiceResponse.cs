using System;

namespace LegalLead.PublicData.Search.Classes
{
    class GetInvoiceResponse
    {
        public string Id { get; set; }

        public string LeadUserId { get; set; }

        public string RequestId { get; set; }

        public string InvoiceNbr { get; set; }

        public string InvoiceUri { get; set; }

        public int? RecordCount { get; set; }

        public decimal? Total { get; set; }

        public DateTime? CompleteDate { get; set; }

        public DateTime? CreateDate { get; set; }
    }
}
