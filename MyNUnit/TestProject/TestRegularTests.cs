using MyNUnit;

namespace TestProject;

public class TestRegularTests
{
    [MyTest]
    public void PassingTest() { }

    [MyTest(typeof(FormatException))]
    public void ExpectedException()
    {
        throw new FormatException();
    }

    [MyTest(typeof(FormatException))]
    public void UnexpectedException()
    {
        throw new InvalidDataException();
    }

    [MyTest(null, "must be ignored")]
    public void IgnoredTest()
    {
        throw new Exception();
    }
}
