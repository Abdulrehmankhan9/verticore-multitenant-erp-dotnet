using System.IdentityModel.Tokens.Jwt;

namespace VertiCore.API.Middleware
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"]
                .FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                var handler = new JwtSecurityTokenHandler();
                if (handler.CanReadToken(token))
                {
                    var jwt = handler.ReadJwtToken(token);
                    var tenantId = jwt.Claims
                        .FirstOrDefault(c => c.Type == "TenantId")?.Value;

                    if (!string.IsNullOrEmpty(tenantId))
                    {
                        context.Items["TenantId"] = tenantId;
                    }
                }
            }

            await _next(context);
        }
    }
}