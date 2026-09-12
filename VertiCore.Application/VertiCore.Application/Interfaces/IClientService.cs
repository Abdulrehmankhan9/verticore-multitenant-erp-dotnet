using VertiCore.Application.DTOs.Client;

namespace VertiCore.Application.Interfaces
{
    public interface IClientService
    {
        Task<List<ClientDto>> GetAllAsync(Guid tenantId);
        Task<ClientDto?> GetByIdAsync(Guid id, Guid tenantId);
        Task<ClientDto> CreateAsync(CreateClientRequest request, Guid tenantId);
        Task UpdateAsync(Guid id, UpdateClientRequest request, Guid tenantId);
        Task DeleteAsync(Guid id, Guid tenantId);
    }
}
