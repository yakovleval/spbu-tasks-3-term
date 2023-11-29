using MyNUnit;

namespace TestProject;

public class TestClass
{
    [Test]
    public void TestPasses() {
        Console.WriteLine("hello world");
    }

    [Test(typeof(FormatException))]
    public void ThrowsCorrectException()
    {
        throw new FormatException();
    }

    [Test(null, "should be ignored")]
    public void IgnoredTest()
    {
        throw new Exception();
    }
}