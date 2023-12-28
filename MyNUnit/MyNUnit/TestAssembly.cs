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

    public ClassReport[] RunTests()
    {
        ClassReport[] results = new ClassReport[_testClasses.Length];
        Parallel.Invoke(_testClasses
            .Select((testClass, index) => new Action(() => results[index] = testClass.RunTests()))
            .ToArray());
        return results;
    }
}
