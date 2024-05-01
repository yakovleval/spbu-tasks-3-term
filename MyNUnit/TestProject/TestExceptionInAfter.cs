using MyNUnit;

namespace TestProject;

public class TestExceptionInAfter
{
    [After]
    public void ThrowsException()
    {
        throw new Exception("exception from 'After' method");
    }

    [MyTest]
    public void RegularTest()
    {
        Console.WriteLine("I shouldn't be printed");
    }
}
