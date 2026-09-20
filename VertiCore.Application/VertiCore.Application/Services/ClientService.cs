using AutoMapper;
using VertiCore.Application.DTOs.Client;
using VertiCore.Application.Interfaces.Repositories;
using VertiCore.Application.Interfaces.Services;
using VertiCore.Domain.Entities;
using VertiCore.Domain.Exceptions;

namespace VertiCore.Application.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;

        public ClientService(IClientRepository clientRepository, IMapper mapper)
        {
            _clientRepository = clientRepository;
            _mapper = mapper;
        }

        public async Task<List<ClientDto>> GetAllAsync(Guid tenantId)
        {
            var clients = await _clientRepository.GetAllAsync();
            var tenantClients = clients.Where(c => c.TenantId == tenantId).ToList();
            return _mapper.Map<List<ClientDto>>(tenantClients);
        }

        public async Task<ClientDto?> GetByIdAsync(Guid id, Guid tenantId)
        {
            var client = await _clientRepository.GetByIdAsync(id);
            if (client == null || client.TenantId != tenantId)
                return null;
            return _mapper.Map<ClientDto>(client);
        }

        public async Task<ClientDto> CreateAsync(CreateClientRequest request, Guid tenantId)
        {
            var client = _mapper.Map<Client>(request);
            client.TenantId = tenantId;
            client.IsActive = true;

            await _clientRepository.AddAsync(client);
            await _clientRepository.SaveChangesAsync();

            return _mapper.Map<ClientDto>(client);
        }

        public async Task UpdateAsync(Guid id, UpdateClientRequest request, Guid tenantId)
        {
            var client = await _clientRepository.GetByIdAsync(id);

            if (client == null || client.TenantId != tenantId)
                throw new ClientNotFoundException(id);

            _mapper.Map(request, client);

            _clientRepository.Update(client);
            await _clientRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id, Guid tenantId)
        {
            var client = await _clientRepository.GetByIdAsync(id);

            if (client == null || client.TenantId != tenantId)
                throw new ClientNotFoundException(id);

            _clientRepository.Delete(client);
            await _clientRepository.SaveChangesAsync();
        }
    }
}