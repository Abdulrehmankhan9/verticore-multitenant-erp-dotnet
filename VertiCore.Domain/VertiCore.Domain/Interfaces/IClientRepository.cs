using VertiCore.Domain.Entities;

namespace VertiCore.Domain.Interfaces
{
    public interface IClientRepository : IRepository<Client>
    {
        Task<List<Client>> SearchAsync(string keyword);
    }
}