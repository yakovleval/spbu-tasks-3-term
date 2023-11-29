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
public record Report(string MethodName, TestResult State, string? Reason = null, long TimeElapsed = 0)
{
    public override string ToString()
    {
        return State switch
        {
            TestResult.PASSED => $"{MethodName} -- OK, time elapsed: {TimeElapsed}",
            TestResult.FAILED =>  $"{MethodName} -- FAILED, reason: {Reason}, time elapsed: {TimeElapsed}",
            _ => $"{MethodName} -- IGNORED, reason: {Reason}"
        };
    }
}

public class TestClass
{
    Type _testClass;
    private MethodInfo[] _beforeClassMethods;
    private MethodInfo[] _afterClassMethods;
    private MethodInfo[] _beforeMethods;
    private MethodInfo[] _afterMethods;
    private MethodInfo[] _testMethods;

    private MethodInfo[] InitArrayOfMethods<TAttribute>(MethodInfo[] methods) where TAttribute : Attribute
    {
        return methods
            .AsEnumerable<MethodInfo>()
            .Where(method => method.GetCustomAttribute<TAttribute>() != null)
            .ToArray();
    }
    public TestClass(Type testClass)
    {
        _testClass = testClass;
        var methods = testClass.GetMethods();
        _beforeClassMethods = InitArrayOfMethods<BeforeClassAttribute>(methods);
        _afterClassMethods = InitArrayOfMethods<AfterClassAttribute>(methods);
        _beforeMethods = InitArrayOfMethods<BeforeAttribute>(methods);
        _afterMethods = InitArrayOfMethods<AfterAttribute>(methods);
        _testMethods = InitArrayOfMethods<TestAttribute>(methods);
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

        RunMethods(_beforeMethods, instance);

        var watch = new Stopwatch();
        Type? exceptionType = null;
        watch.Start();
        try
        {
            method.Invoke(_testClass, null);
        }
        catch (Exception e)
        {
            exceptionType = e.GetType();
        }
        watch.Stop();

        RunMethods(_afterMethods, instance);

        if (exceptionType == attr.Expected)
        {
            return new Report(method.Name, TestResult.PASSED, null, watch.ElapsedMilliseconds);
        }
        else if (attr.Expected is null)
        {
            return new Report(method.Name, TestResult.FAILED, $"unexpected exception: {exceptionType}", watch.ElapsedMilliseconds);
        }
        else
        {
            return new Report(method.Name, TestResult.FAILED, $"expected: {attr.Expected}, but was: {exceptionType}", watch.ElapsedMilliseconds)
        }
    }

    public Report[] RunTests()
    {
        var reports = new Report[_testMethods.Length];
        RunMethods(_beforeClassMethods, _testClass);
        var instance = Activator.CreateInstance(_testClass)!;
        Parallel.Invoke(_testMethods
            .Select((method, index) => new Action(() => reports[index] = RunTest(method, instance)))
            .ToArray());
        RunMethods(_afterClassMethods, _testClass);
        return reports;
    }

    private void RunMethods(MethodInfo[] methods, object methodsObject)
    {
        Parallel.Invoke(methods
            .Select(method => new Action(() => method.Invoke(methodsObject, null)))
            .ToArray());
    }
}
