namespace VertiCore.Application.DTOs.User
{
    public class SetPasswordRequest
    {
        public string Token { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}