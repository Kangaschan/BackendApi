namespace LRA.Common.Exceptions;

public class Required2FaException : Exception
{
    public Required2FaException(string message) : base(message) {}
}
