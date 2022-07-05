namespace BuildingBlocks.Web.Exceptions;

public class ResourceNotFoundException : ApiException
{
    public ResourceNotFoundException()
    {
    }

    public ResourceNotFoundException(string? message) : base(message)
    {
    }
}