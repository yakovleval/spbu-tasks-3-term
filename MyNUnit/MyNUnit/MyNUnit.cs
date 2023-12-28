using System.Reflection;

namespace MyNUnit;

public class MyNUnit
{
    private List<TestClass> _testClasses;
    public MyNUnit(string path)
    {
        var directory = Directory
            .EnumerateFiles(path, "*.dll")
            .Where(file => !file.EndsWith("MyNUnit.dll"))
            .ToList();
        directory.Sort();
        var testAssemblies = directory
            .Select(Assembly.LoadFrom)
            .ToList();
        _testClasses = testAssemblies
            .SelectMany(a => a.ExportedTypes
                             .Select(type => new TestClass(type))
                             .ToArray())
            .ToList();
    }

    public void RunTestsAndPrintResult()
    {
        ClassReport[] result = new ClassReport[_testClasses.Count];
        Parallel.Invoke(_testClasses
            .Select((testClass, index) => new Action(() => result[index] = testClass.RunTests()))
            .ToArray());
        PrintResult(result);
    }

    private void PrintResult(ClassReport[] result)
    {
        foreach (var classReport in result)
        {
            Console.WriteLine(classReport);
        }
    }
}
