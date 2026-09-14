namespace VertiCore.Domain.Exceptions
{
    public class ClientNotFoundException : Exception
    {
        public ClientNotFoundException(Guid id)
            : base($"Client with ID {id} was not found") { }
    }
}