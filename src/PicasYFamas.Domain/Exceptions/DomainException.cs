using System;

namespace PicasYFamas.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}
