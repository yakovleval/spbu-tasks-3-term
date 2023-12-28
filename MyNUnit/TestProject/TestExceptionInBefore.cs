using MyNUnit;

namespace TestProject;

public class TestExceptionInBefore
{
    [Before]
    public void ThrowsException()
    {
        throw new FormatException("exception from 'Before' method");
    }

    [MyTest]
    public void RegularTest() { }
}
