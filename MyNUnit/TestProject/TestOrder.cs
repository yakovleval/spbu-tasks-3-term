using MyNUnit;
using System.Collections.Concurrent;

namespace TestProject;

public class TestOrder
{
    public static readonly ConcurrentQueue<string> ORDER = new();

    [BeforeClass]
    public static void BeforeClass1()
    {
        ORDER.Enqueue("BeforeClass");


    }

    [BeforeClass]
    public static void BeforeClass2()
    {
        ORDER.Enqueue("BeforeClass");
    }

    [Before]
    public void Before1()
    {
        ORDER.Enqueue("Before");
    }

    [Before]
    public void Before2()
    {
        ORDER.Enqueue("Before");
    }

    [MyTest]
    public void Test()
    {
        ORDER.Enqueue("MyTest");
    }

    [After]
    public void After1()
    {
        ORDER.Enqueue("After");
    }

    [After]
    public void After2()
    {
        ORDER.Enqueue("After");
    }

    [AfterClass]
    public static void AfterClass1()
    {
        ORDER.Enqueue("AfterClass");
    }

    [AfterClass]
    public static void AfterClass2()
    {
        ORDER.Enqueue("AfterClass");
    }
}