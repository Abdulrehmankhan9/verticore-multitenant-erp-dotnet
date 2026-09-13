using Microsoft.AspNetCore.Http;
using VertiCore.Application.Interfaces;

namespace VertiCore.Application.Services
{
    public class CurrentTenantService : ICurrentTenantService
    {
        public Guid? TenantId { get; }

        public CurrentTenantService(IHttpContextAccessor httpContextAccessor)
        {
            var tenantIdStr = httpContextAccessor.HttpContext?.Items["TenantId"]?.ToString();
            if (!string.IsNullOrEmpty(tenantIdStr))
                TenantId = Guid.Parse(tenantIdStr);
        }
    }
}