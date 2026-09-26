namespace VertiCore.Domain.Exceptions
{
    public class DuplicateUserEmailException : Exception
    {
        public DuplicateUserEmailException()
            : base("An account with this email already exists")
        {
        }
    }
}