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
        ClassReport[][] result = new ClassReport[_testAssemblies.Count][];
        Parallel.Invoke(_testAssemblies
            .Select((testAssembly, index) => new Action(() => result[index] = testAssembly.RunTests()))
            .ToArray());
        PrintResult(result);
    }

    private void PrintResult(ClassReport[][] result)
    {
        foreach (var assemblyResult in result)
        {
            foreach (var classReport in assemblyResult)
            {
                Console.WriteLine(classReport);
            }
        }
    }
}
