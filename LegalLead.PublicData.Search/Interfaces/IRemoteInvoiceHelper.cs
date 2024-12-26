namespace LegalLead.PublicData.Search.Interfaces
{
    public interface IRemoteInvoiceHelper
    {
        string GetInvoicesByCustomerId();
        string CreateInvoice(string invoiceData);
    }
}
