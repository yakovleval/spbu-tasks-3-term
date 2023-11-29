using System.Collections.Concurrent;
using System.Reflection;

namespace MyNUnit;

public class TestAssembly
{
    private List<TestClass> _testClasses;

    public TestAssembly(Assembly assembly)
    {
        _testClasses = assembly
            .ExportedTypes
            .Select(type => new TestClass(type))
            .ToList();
    }

    public List<ClassReport> RunTests()
    {
        ConcurrentBag<ClassReport> results = new();
        Parallel.Invoke(_testClasses
            .Select(testClass => new Action(() => results.Add(testClass.RunTests())))
            .ToArray());
        return results.ToList();
    }
}
