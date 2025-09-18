namespace LRA.Common.Exceptions;

public class TokenOutdatedException : Exception
{
    public TokenOutdatedException(string message) : base(message) {}
}
