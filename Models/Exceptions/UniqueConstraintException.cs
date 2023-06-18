using System.Runtime.Serialization;

namespace Models.Exceptions;

[Serializable]
public class UniqueConstraintException : Exception
{
    public UniqueConstraintException()
    {
    }

    public UniqueConstraintException(string? message) : base(message)
    {
    }
}