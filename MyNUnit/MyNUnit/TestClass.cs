using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;

namespace MyNUnit;

public enum TestResult
{
    PASSED,
    FAILED,
    IGNORED
}
public record Report(string MethodName, TestResult state, string? reason = null, long timeElapsed = 0);

public class TestClass
{
    Type _testClass;
    private List<MethodInfo> _beforeClassMethods;
    private List<MethodInfo> _afterClassMethods;
    private List<MethodInfo> _beforeMethods;
    private List<MethodInfo> _afterMethods;
    private List<MethodInfo> _testMethods;
    public TestClass(Type testClass)
    {
        _testClass = testClass;
        var methods = testClass.GetMethods();
        _beforeClassMethods = methods
            .AsEnumerable<MethodInfo>()
            .Where(method => method.GetCustomAttribute<BeforeClassAttribute>() != null)
            .ToList();
        _afterClassMethods = methods
            .AsEnumerable<MethodInfo>()
            .Where(method => method.GetCustomAttribute<AfterClassAttribute>() != null)
            .ToList();
        _beforeMethods = methods
            .AsEnumerable<MethodInfo>()
            .Where(method => method.GetCustomAttribute<BeforeAttribute>() != null)
            .ToList();
        _afterMethods = methods
            .AsEnumerable<MethodInfo>()
            .Where(method => method.GetCustomAttribute<AfterAttribute>() != null)
            .ToList();
        _testMethods = methods
            .AsEnumerable<MethodInfo>()
            .Where(method => method.GetCustomAttribute<TestAttribute>() != null)
            .ToList();
        foreach (var method in _beforeClassMethods)
        {
            if (!method.IsStatic)
            {
                throw new CustomAttributeFormatException($"{method.Name} must be static");
            }
        }
        foreach (var method in _afterClassMethods)
        {
            if (!method.IsStatic)
            {
                throw new CustomAttributeFormatException($"{method.Name} must be static");
            }
        }
    }

    public Report RunTest(MethodInfo method, object instance)
    {
        var attr = method.GetCustomAttribute<TestAttribute>()!;
        if (attr.Ignore is not null)
        {
            return new Report(method.Name, TestResult.IGNORED, attr.Ignore);
        }
        Parallel.Invoke(_beforeMethods
            .Select(m => new Action(() => m.Invoke(instance, null)))
            .ToArray());
        Report report;
        var watch = new Stopwatch();
        try
        {
            watch.Start();
            method.Invoke(_testClass, null);
            watch.Stop();
        }
        catch (MyAssertionException e)
        {
            report = new Report(method.Name, TestResult.FAILED, e.Message);
        }
        catch (Exception e)
        {
            if (attr.Expected is null)
            {
                report = new Report(method.Name, TestResult.FAILED, $"unexpected exception: {e.GetType()}");
            }
            if (attr.Expected != e.GetType())
            {
                report = new Report(method.Name, TestResult.FAILED, $"expected exception: {attr.Expected}, but was: {e.GetType()}");
            }
        }
        report = new Report(method.Name, TestResult.PASSED, null, watch.ElapsedMilliseconds);
        Parallel.Invoke(_afterMethods
            .Select(m => new Action(() => m.Invoke(instance, null)))
            .ToArray());
        return report;
    }

    public List<Report> RunTests()
    {
        ConcurrentBag<Report> reports = new();

        Parallel.Invoke(_beforeClassMethods
            .Select(method => new Action(() => method.Invoke(_testClass, null)))
            .ToArray());

        var instance = Activator.CreateInstance(_testClass)!;

        Parallel.Invoke(_testMethods
            .Select(method => new Action(() => reports.Add(RunTest(method, instance))))
            .ToArray());

        Parallel.Invoke(_afterClassMethods
            .Select(method => new Action(() => method.Invoke(_testClass, null)))
            .ToArray());

        return reports.ToList();
    }
}
