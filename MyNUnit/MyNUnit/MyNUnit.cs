using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.Loader;

namespace MyNUnit;

/// <summary>
/// Class with methods for running tests and printing results
/// </summary>
public class MyNUnit : IDisposable
{
    private List<TestAssembly> _testAssemblies = new();
    private AssemblyLoadContext _context;

    /// <summary>
    /// creates an instance of 'MyNUnit' class
    /// </summary>
    /// <param name="path">path to a directory which contains 
    /// assemblies with test classes</param>
    public MyNUnit(string dirPath)
    {
        _context = new AssemblyLoadContext("DisposableContext", true);
        var dirInfo = new DirectoryInfo(dirPath);
        foreach (var fileInfo in dirInfo.GetFiles())
        {
            if (!fileInfo.Name.Contains(".dll"))
            {
                continue;
            }
            using var fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read);
            var assembly = _context.LoadFromStream(fs);
            _testAssemblies.Add(new TestAssembly(assembly));
        }
    }

    public void Dispose()
    {
        _context.Unload();
    }

    public AssemblyReport[] RunTests()
    {
        ConcurrentBag<AssemblyReport> assemblyReports = new();
        Parallel.Invoke(_testAssemblies
            .Select(testAssembly => new Action(() =>
            {
                var assemblyReport = testAssembly.RunTests();
                if (assemblyReport.classReports.Length > 0)
                {
                    assemblyReports.Add(assemblyReport);
                }
            }))
            .ToArray());
        return assemblyReports.ToArray();
    }

    private void PrintResult(ClassReport[] result)
    {
        foreach (var classReport in result)
        {
            Console.WriteLine(classReport);
        }
    }
}
