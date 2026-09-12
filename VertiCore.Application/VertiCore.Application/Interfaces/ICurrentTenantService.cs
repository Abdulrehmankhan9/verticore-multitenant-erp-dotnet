namespace VertiCore.Application.Interfaces
{
    public interface ICurrentTenantService
    {
        Guid? TenantId { get; }
    }
}
