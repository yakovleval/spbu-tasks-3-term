namespace Test1attempt1.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [TestCase("../../../TestFolder1")]
    [TestCase("../../../TestFolder1/EmptyFile.txt")]
    [TestCase("../../../../Test1attempt1")]
    public void TestCheckSum(string path)
    {
        var result = CheckSum.Evaluate(path);
        var resultParallel = CheckSum.EvaluateParallel(path).Result;
        Assert.That(result.SequenceEqual(resultParallel), Is.True);
    }

    [TestCase("")]
    [TestCase("nonexistentpath")]
    [TestCase("../../../nonexistentFolder")]
    public void TestInvalidPathAsync(string path)
    {
        Assert.Throws<ArgumentException>(() => CheckSum.Evaluate(path));
        Assert.Throws<AggregateException>(() => _ = CheckSum.EvaluateParallel(path).Result);
    }
}