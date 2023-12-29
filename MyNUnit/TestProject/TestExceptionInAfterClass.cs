using MyNUnit;

namespace TestProject;

public class TestExceptionInAfterClass
{
    [AfterClass]
    public static void ThrowsException()
    {
        throw new InvalidDataException("exception from 'AfterClass' method");
    }

    [MyTest]
    public void RegularTest() { }
}
