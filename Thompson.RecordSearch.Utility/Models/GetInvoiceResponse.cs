using System;
using System.Collections.Generic;

namespace Thompson.RecordSearch.Utility.Models
{
    public class GetInvoiceResponse
    {
        public List<InvoiceHeaderModel> Headers { get; set; } = new List<InvoiceHeaderModel>();
        public List<InvoiceDetailModel> Lines { get; set; } = new List<InvoiceDetailModel>();
    }

    public class InvoiceHeaderModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string LeadUserId { get; set; }
        public string RequestId { get; set; }
        public string InvoiceNbr { get; set; }
        public string InvoiceUri { get; set; }
        public int RecordCount { get; set; }
        public decimal? InvoiceTotal { get; set; }
        public DateTime? CompleteDate { get; set; }
        public DateTime? CreateDate { get; set; }
    }
    public class InvoiceDetailModel
    {
        public string Id { get; set; }
        public int LineNbr { get; set; }
        public string Description { get; set; }
        public int ItemCount { get; set; }
        public decimal? ItemPrice { get; set; }
        public decimal? ItemTotal { get; set; }
    }
}
