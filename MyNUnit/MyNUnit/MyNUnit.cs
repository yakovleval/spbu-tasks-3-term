using System.Reflection;

namespace MyNUnit;

/// <summary>
/// Class with methods for running tests and printing results
/// </summary>
public class MyNUnit
{
    private List<TestClass> _testClasses;
    private Assembly _testAssembly;

    /// <summary>
    /// creates an instance of 'MyNUnit' class
    /// </summary>
    /// <param name="path">path to a directory which contains 
    /// assemblies with test classes</param>
    public MyNUnit(string path)
    {
        //var directory = Directory
        //    .EnumerateFiles(path, "*.dll")
        //    .Where(file => !file.EndsWith("MyNUnit.dll"))
        //    .ToList();
        //var testAssemblies = directory
        //    .Select(Assembly.LoadFrom)
        //    .ToList();
        //_testClasses = testAssemblies
        //    .SelectMany(a => a.ExportedTypes
        //                     .Select(type => new TestClass(type))
        //                     .ToArray())
        //    .ToList();
        _testAssembly = Assembly.LoadFrom(path);
        _testClasses = _testAssembly.ExportedTypes
                             .Select(type => new TestClass(type))
                             .ToList();
    }

    public AssemblyReport RunTests()
    {
        ClassReport[] result = new ClassReport[_testClasses.Count];
        Parallel.Invoke(_testClasses
            .Select((testClass, index) => new Action(() => result[index] = testClass.RunTests()))
            .ToArray());
        return new AssemblyReport(_testAssembly.ManifestModule.Name, result);
    }

    /// <summary>
    /// runs all tests in the directory and prints result of them
    /// </summary>
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
