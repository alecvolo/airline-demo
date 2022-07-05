namespace BuildingBlocks.Web.Exceptions;

public class ResourceForbiddenException : ApiException
{
    public ResourceForbiddenException()
    {
    }

    public ResourceForbiddenException(string? message) : base(message)
    {
    }
}