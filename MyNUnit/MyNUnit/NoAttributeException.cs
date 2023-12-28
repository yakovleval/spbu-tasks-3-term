namespace MyNUnit;

public class NoAttributeException : Exception
{
    public NoAttributeException(string message = "") : base(message) { }
}
