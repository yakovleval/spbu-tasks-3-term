using System.Collections.Concurrent;
using System.Reflection;

namespace MyNUnit;

public class TestAssembly
{
    private List<TestClass> _testClasses = new();
    private readonly string _assemblyName;
    public TestAssembly(Assembly assembly)
    {
        _testClasses = assembly.ExportedTypes
                             .Where(c => ClassContainsTests(c))
                             .Select(type => new TestClass(type))
                             .ToList();
        _assemblyName = assembly.ManifestModule.ScopeName;
    }

    private bool ClassContainsTests(Type classToCheck)
    {
        var methods = classToCheck.GetMethods();
        return methods
            .AsEnumerable<MethodInfo>()
            .Where(method => method.GetCustomAttribute<MyTestAttribute>() != null)
            .ToArray()
            .Length > 0;
    }

    public AssemblyReport RunTests()
    {
        ConcurrentBag<ClassReport> classReports = new();
        Parallel.Invoke(_testClasses
            .Select(testClass => new Action(() =>
            {
                var classReport = testClass.RunTests();
                classReports.Add(classReport);
            }))
            .ToArray());
        return new AssemblyReport(_assemblyName, classReports.ToArray());
    }
}
