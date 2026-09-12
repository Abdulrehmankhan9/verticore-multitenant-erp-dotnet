using VertiCore.Domain.Entities;

namespace VertiCore.Application.Interfaces
{
    public interface IClientRepository : IRepository<Client>
    {
        Task<List<Client>> SearchAsync(string keyword);
    }
}
