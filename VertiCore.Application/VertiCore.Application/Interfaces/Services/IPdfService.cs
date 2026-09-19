namespace VertiCore.Application.Interfaces.Services
{
    public interface IPdfService
    {
        byte[] GenerateInvoicePdf(Guid invoiceId);
    }
}

