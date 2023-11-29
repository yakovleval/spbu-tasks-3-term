using System.Collections.Concurrent;
using System.Reflection;

namespace MyNUnit;

public class TestAssembly
{
    private TestClass[] _testClasses;

    public TestAssembly(Assembly assembly)
    {
        _testClasses = assembly
            .ExportedTypes
            .Select(type => new TestClass(type))
            .ToArray();
    }

    public List<Report> RunTests()
    {
        List<(int, Report[])> results = new();
        Parallel.Invoke(_testClasses
            .Select((testClass, index) => new Action(() => results.Add((index, testClass.RunTests()))))
            .ToArray());
        results.Sort();
        List<Report> assemblyReport = results
            .Select(pair => pair.Item2)
            .SelectMany(report => report)
            .ToList();
        return assemblyReport;
    }
}
