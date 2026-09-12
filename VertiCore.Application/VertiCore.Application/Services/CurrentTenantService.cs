using Microsoft.AspNetCore.Http;
using VertiCore.Application.Interfaces;

namespace VertiCore.Infrastructure.Services
{
    public class CurrentTenantService : ICurrentTenantService
    {
        public Guid? TenantId { get; }

        public CurrentTenantService(IHttpContextAccessor httpContextAccessor)
        {
            var claim = httpContextAccessor.HttpContext?.User?.FindFirst("TenantId")?.Value;
            if (!string.IsNullOrEmpty(claim))
                TenantId = Guid.Parse(claim);
        }
    }
}
