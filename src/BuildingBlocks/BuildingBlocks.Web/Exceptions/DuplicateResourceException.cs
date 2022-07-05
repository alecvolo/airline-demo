namespace BuildingBlocks.Web.Exceptions;

public class DuplicateResourceException : ApiException
{
    public DuplicateResourceException()
    {
    }

    public DuplicateResourceException(string? message) : base(message)
    {
    }
}