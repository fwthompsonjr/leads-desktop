namespace LegalLead.PublicData.Search.Interfaces
{
    public interface IRemoteInvoiceHelper
    {
        string GetInvoicesByCustomerId();
        string CreateInvoice(string invoiceData);
        string PreviewInvoice(string invoiceData);
        void UpdateExcelStatus(string json);
    }
}
