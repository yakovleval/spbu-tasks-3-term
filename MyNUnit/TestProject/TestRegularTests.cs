using MyNUnit;

namespace TestProject;

public class TestRegularTests
{
    [MyTest]
    public int PassingTest()
    {
        return 0;
    }

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
