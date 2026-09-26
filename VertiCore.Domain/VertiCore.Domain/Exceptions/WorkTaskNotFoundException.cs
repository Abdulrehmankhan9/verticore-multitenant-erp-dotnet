namespace VertiCore.Domain.Exceptions
{
    public class WorkTaskNotFoundException : Exception
    {
        public WorkTaskNotFoundException(Guid id)
            : base($"Task with ID {id} was not found")
        {
        }
    }
}