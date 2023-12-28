namespace MyNUnit;

public class TestRunFailureException : Exception
{
    public TestRunFailureException(string message) : base(message) { }
}
