using BCrypt.Net;
using System.Security.Cryptography;
using System.Text;
using VertiCore.Application.DTOs.Auth;
using VertiCore.Application.Interfaces.Services;
using VertiCore.Application.Interfaces.Repositories;
using VertiCore.Domain.Entities;
using VertiCore.Domain.Enums;
using VertiCore.Domain.Exceptions;

namespace VertiCore.Application.Services
{
    public class AuthService : IAuthService
    {
        private static readonly TimeSpan PasswordResetLifetime = TimeSpan.FromMinutes(30);
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;

        public AuthService(
            IRepository<Tenant> tenantRepository,
            IUserRepository userRepository,
            IJwtService jwtService,
            IEmailService emailService)
        {
            _tenantRepository = tenantRepository;
            _userRepository = userRepository;
            _jwtService = jwtService;
            _emailService = emailService;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var tenant = new Tenant
            {
                Name = request.TenantName,
                ContactEmail = request.Email.Trim().ToLowerInvariant(),
                IsActive = true
            };
            if (await _userRepository.GetByEmailAsync(request.Email) != null)
                throw new DuplicateUserEmailException();

            await _tenantRepository.AddAsync(tenant);
            await _tenantRepository.SaveChangesAsync();

            var user = new User
            {
                TenantId = tenant.Id,
                FullName = request.FullName,
                Email = request.Email.Trim().ToLowerInvariant(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = UserRole.TenantAdmin,
                IsActive = true
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            // Email bhejo
            await _emailService.SendWelcomeEmailAsync(
                request.Email,
                request.TenantName,
                request.FullName
            );

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
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null || !user.IsActive || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new InvalidCredentialsException();

            var token = _jwtService.GenerateToken(user);

            return new AuthResponse
            {
                Token = token,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }

        public async Task RequestPasswordResetAsync(ForgotPasswordRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email.Trim().ToLowerInvariant());
            if (user == null || !user.IsActive)
                return;

            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');

            user.PasswordResetTokenHash = HashResetToken(token);
            user.PasswordResetExpiresAt = DateTime.UtcNow.Add(PasswordResetLifetime);
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            try
            {
                await _emailService.SendPasswordResetEmailAsync(user.Email, user.FullName, token);
            }
            catch
            {
                user.PasswordResetTokenHash = null;
                user.PasswordResetExpiresAt = null;
                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();
            }
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var tokenHash = HashResetToken(request.Token);
            var user = await _userRepository.GetByPasswordResetTokenHashAsync(tokenHash);

            if (user == null || !user.IsActive || user.PasswordResetExpiresAt <= DateTime.UtcNow)
                return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            user.PasswordResetTokenHash = null;
            user.PasswordResetExpiresAt = null;
            user.UpdatedAt = DateTime.UtcNow;
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
            return true;
        }

        private static string HashResetToken(string token)
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return Convert.ToHexString(hash);
        }
    }
}