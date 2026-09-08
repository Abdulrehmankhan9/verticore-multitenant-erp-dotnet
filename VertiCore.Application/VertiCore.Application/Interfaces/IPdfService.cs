namespace VertiCore.Application.Interfaces
{
    public interface IPdfService
    {
        byte[] GenerateInvoicePdf(Guid invoiceId);
    }
}