namespace LegalLead.PublicData.Search.Interfaces
{
    public interface IRemoteInvoiceHelper
    {
        string GetInvoicesByCustomerId();
        string CreateInvoice(string invoiceData);
        string PreviewInvoice(string invoiceData);
        void UpdateExcelStatus(string json);
        string GetInvoiceStatus(string invoiceData);
        string GetBillingCode();
        string SetBillingCode(string leadId, string billingMode);
        string GetInvoicesByTrackingId(string customerId, string trackingId);
    }
}
