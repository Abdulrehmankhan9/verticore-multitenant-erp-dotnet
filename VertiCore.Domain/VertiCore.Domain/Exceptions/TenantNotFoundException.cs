namespace VertiCore.Domain.Exceptions
{
    public class TenantNotFoundException : Exception
    {
        public TenantNotFoundException(Guid id)
            : base($"Tenant with ID {id} was not found") { }
    }
}