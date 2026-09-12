using VertiCore.Application.DTOs.Client;
using VertiCore.Application.Interfaces;
using VertiCore.Domain.Entities;
using VertiCore.Application.Interfaces;

namespace VertiCore.Application.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<List<ClientDto>> GetAllAsync(Guid tenantId)
        {
            var clients = await _clientRepository.GetAllAsync();
            var tenantClients = clients.Where(c => c.TenantId == tenantId).ToList();

            return tenantClients.Select(c => new ClientDto
            {
                Id = c.Id,
                FullName = c.FullName,
                Email = c.Email,
                Phone = c.Phone,
                Address = c.Address,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt
            }).ToList();
        }

        public async Task<ClientDto?> GetByIdAsync(Guid id, Guid tenantId)
        {
            var client = await _clientRepository.GetByIdAsync(id);

            if (client == null || client.TenantId != tenantId)
                return null;

            return new ClientDto
            {
                Id = client.Id,
                FullName = client.FullName,
                Email = client.Email,
                Phone = client.Phone,
                Address = client.Address,
                IsActive = client.IsActive,
                CreatedAt = client.CreatedAt
            };
        }

        public async Task<ClientDto> CreateAsync(CreateClientRequest request, Guid tenantId)
        {
            var client = new Client
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _clientRepository.AddAsync(client);
            await _clientRepository.SaveChangesAsync();

            return new ClientDto
            {
                Id = client.Id,
                FullName = client.FullName,
                Email = client.Email,
                Phone = client.Phone,
                Address = client.Address,
                IsActive = client.IsActive,
                CreatedAt = client.CreatedAt
            };
        }

        public async Task UpdateAsync(Guid id, UpdateClientRequest request, Guid tenantId)
        {
            var client = await _clientRepository.GetByIdAsync(id);

            if (client == null || client.TenantId != tenantId)
                throw new KeyNotFoundException("Client not found");

            client.FullName = request.FullName;
            client.Email = request.Email;
            client.Phone = request.Phone;
            client.Address = request.Address;
            client.IsActive = request.IsActive;

            _clientRepository.Update(client);
            await _clientRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id, Guid tenantId)
        {
            var client = await _clientRepository.GetByIdAsync(id);

            if (client == null || client.TenantId != tenantId)
                throw new KeyNotFoundException("Client not found");

            _clientRepository.Delete(client);
            await _clientRepository.SaveChangesAsync();
        }
    }
}
