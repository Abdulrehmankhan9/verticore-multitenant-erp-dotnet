using VertiCore.Domain.Enums;

namespace VertiCore.Application.DTOs.User
{
    public class InviteUserRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; }
    }
}