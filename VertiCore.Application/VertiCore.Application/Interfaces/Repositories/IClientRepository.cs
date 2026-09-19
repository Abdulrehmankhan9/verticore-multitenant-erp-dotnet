using VertiCore.Domain.Entities;

namespace VertiCore.Application.Interfaces.Repositories
{
    public interface IClientRepository : IRepository<Client>
    {
        Task<List<Client>> SearchAsync(string keyword);
    }
}


