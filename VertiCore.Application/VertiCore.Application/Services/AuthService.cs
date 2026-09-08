using VertiCore.Application.DTOs.Auth;
using VertiCore.Application.Interfaces;
using VertiCore.Domain.Entities;
using VertiCore.Domain.Enums;
using VertiCore.Domain.Interfaces;

namespace VertiCore.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<User> _userRepository;

        public AuthService(IRepository<Tenant> tenantRepository, IRepository<User> userRepository)
        {
            _tenantRepository = tenantRepository;
            _userRepository = userRepository;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Name = request.TenantName,
                ContactEmail = request.Email,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _tenantRepository.AddAsync(tenant);
            await _tenantRepository.SaveChangesAsync();

            var user = new User
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = UserRole.TenantAdmin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return new AuthResponse
            {
                Token = "temp-token",
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            throw new NotImplementedException();
        }
    }
}