using System.Collections.Immutable;
using System.Reflection;

namespace MyNUnit;

public class MyNUnit
{
    private List<TestAssembly> _testAssemblies;
    public MyNUnit(string path)
    {
        var directory = Directory
            .EnumerateFiles(path, "*.dll")
            .Where(file => !file.EndsWith("MyNUnit.dll"))
            .ToList();
        directory.Sort();
        _testAssemblies = directory
            .Select(Assembly.LoadFrom)
            .Select(a => new TestAssembly(a))
            .ToList();
    }

    public void RunTestsAndPrintResult()
    {
        List<List<Report>> result = new();
        Parallel.Invoke(_testAssemblies
            .Select(testClass => new Action(() => result.Add(testClass.RunTests())))
            .ToArray());
        PrintResult(result);
    }

    private void PrintResult(List<List<Report>> result)
    {
        foreach (var assemblyResult in result)
        {
            foreach (var report in assemblyResult)
            {
                Console.WriteLine(report);
            }
        }
    }
}
