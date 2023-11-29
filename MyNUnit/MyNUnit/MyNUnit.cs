using System.Collections.Immutable;
using System.Reflection;

namespace MyNUnit;

public class MyNUnit
{
    private List<TestAssembly>? _testAssemblies;
    public MyNUnit(string path)
    {
        _testAssemblies = Directory
            .EnumerateFiles(path, "*.dll")
            .Select(Assembly.Load)
            .Select(a => new TestAssembly(a))
            .ToList();
    }
}
