using MyNUnit;

namespace TestProject;

public class TestExceptionInBeforeClass
{
    [BeforeClass]
    public static void ThrowsException()
    {
        throw new Exception("exception from 'BeforeClass' method");
    }

    [MyTest(typeof(InvalidDataException))]
    public void RegularTest()
    {
        throw new Exception();
    }
}
