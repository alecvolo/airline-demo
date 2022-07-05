using System;

namespace BuildingBlocks.Web.Exceptions;

public class ApiException : ApplicationException
{
    public ApiException()
    {
    }

    public ApiException(string? message)
        : base(message)
    {
    }

    public ApiException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}