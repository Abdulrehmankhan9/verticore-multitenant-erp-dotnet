namespace VertiCore.Application.Interfaces.Services
{
    public interface ICurrentTenantService
    {
        Guid? TenantId { get; }
    }
}

