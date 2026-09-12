using BCrypt.Net;
using VertiCore.Application.DTOs.Auth;
using VertiCore.Application.Interfaces;
using VertiCore.Domain.Entities;
using VertiCore.Domain.Enums;
using VertiCore.Application.Interfaces;

namespace VertiCore.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IJwtService _jwtService;

        public AuthService(
            IRepository<Tenant> tenantRepository,
            IRepository<User> userRepository,
            IJwtService jwtService)
        {
            _tenantRepository = tenantRepository;
            _userRepository = userRepository;
            _jwtService = jwtService;
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

            var token = _jwtService.GenerateToken(user);

            return new AuthResponse
            {
                Token = token,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var users = await _userRepository.GetAllAsync();
            var user = users.FirstOrDefault(u => u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            var token = _jwtService.GenerateToken(user);

            return new AuthResponse
            {
                Token = token,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }
    }
}
