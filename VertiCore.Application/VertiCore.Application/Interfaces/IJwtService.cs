using VertiCore.Domain.Entities;

namespace VertiCore.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}