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
    public void RegularTest() { }

    [MyTest]
    public void RegularTest2() { }

    [MyTest]
    public void RegularTest3() { }
}
