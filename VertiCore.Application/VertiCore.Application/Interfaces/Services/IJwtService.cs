using VertiCore.Domain.Entities;

namespace VertiCore.Application.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}

