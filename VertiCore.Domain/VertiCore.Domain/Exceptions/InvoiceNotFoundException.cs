namespace VertiCore.Domain.Exceptions
{
    public class InvoiceNotFoundException : Exception
    {
        public InvoiceNotFoundException(Guid id)
            : base($"Invoice with ID {id} was not found") { }
    }
}