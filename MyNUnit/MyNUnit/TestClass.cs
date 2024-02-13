using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace MyNUnit;

/// <summary>
/// class which represents test class
/// </summary>
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
    
    /// <summary>
    /// creates an instance of TestClass class
    /// </summary>
    /// <param name="testClass">'Type' object of the test class</param>
    /// <exception cref="CustomAttributeFormatException">thrown if one of the 
    /// 'BeforeClass' or 'AfterClass' methods is not static</exception>
    public TestClass(Type testClass)
    {
        _testClass = testClass;
        var methods = testClass.GetMethods();
        _beforeClassMethods = InitArrayOfMethods<BeforeClassAttribute>(methods);
        _afterClassMethods = InitArrayOfMethods<AfterClassAttribute>(methods);
        _beforeMethods = InitArrayOfMethods<BeforeAttribute>(methods);
        _afterMethods = InitArrayOfMethods<AfterAttribute>(methods);
        _testMethods = InitArrayOfMethods<MyTestAttribute>(methods);
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

    private string GenerateSideMethodsFailureMessage(List<(string, TargetInvocationException)> result)
    {
        var message = new StringBuilder();
        message.Append("unhandled exceptions occured in side methods:\n");
        foreach (var (methodName, exception) in result)
        {
            message.Append($"{methodName}(): " +
                $"{exception.InnerException!.GetType().Name}: " +
                $"{exception.InnerException!.Message}");
        }
        return message.ToString();
    }

    /// <summary>
    /// runs a given test from the class
    /// </summary>
    /// <param name="method">object representing the method</param>
    /// <param name="instance">instance of the test class</param>
    /// <returns>result of the test</returns>
    public TestReport RunTest(MethodInfo method, object instance)
    {
        var attribute = method.GetCustomAttribute<MyTestAttribute>()!;
        if (attribute.Ignore is not null)
        {
            return new TestReport(method.Name,
                TestResult.IGNORED,
                null,
                null,
                attribute.Ignore);
        }

        var result = RunMethods(_beforeMethods, instance);
        if (result.Count > 0)
        {
            var reason = GenerateSideMethodsFailureMessage(result);
            return new TestReport(method.Name,
                TestResult.IGNORED,
                null,
                null,
                reason);
        }

        var watch = new Stopwatch();
        Type? exceptionType = null;
        watch.Start();
        try
        {
            method.Invoke(instance, null);
        }
        catch (TargetInvocationException e)
        {
            exceptionType = e.InnerException!.GetType();
        }
        watch.Stop();

        result = RunMethods(_afterMethods, instance);
        if (result.Count > 0)
        {
            var reason = GenerateSideMethodsFailureMessage(result);
            return new TestReport(method.Name,
                TestResult.IGNORED,
                null, null,
                reason);
        }

        if (exceptionType == attribute.Expected)
        {
            return new TestReport(method.Name,
                TestResult.PASSED,
                exceptionType,
                attribute.Expected,
                null,
                watch.ElapsedMilliseconds);
        }
        else
        {
            return new TestReport(method.Name,
                TestResult.FAILED,
                exceptionType,
                attribute.Expected,
                null,
                watch.ElapsedMilliseconds);
        }
    }

    /// <summary>
    /// runs all tests from the test class
    /// </summary>
    /// <returns>result of all tests in the test class</returns>
    public ClassReport RunTests()
    {
        var reports = new TestReport[_testMethods.Length];
        var result = RunMethods(_beforeClassMethods, null);
        if (result.Count > 0)
        {
            var reason = GenerateSideMethodsFailureMessage(result);
            return new ClassReport(_testClass.Name,
                ClassResult.FAILED,
                null,
                reason);

        }
        var instance = Activator.CreateInstance(_testClass)!;
        Parallel.Invoke(_testMethods
            .Select((method, index) => new Action(() =>
            {
                reports[index] = RunTest(method, instance);
            }))
            .ToArray());
        result = RunMethods(_afterClassMethods, null);
        if (result.Count > 0)
        {
            var reason = GenerateSideMethodsFailureMessage(result);
            return new ClassReport(_testClass.Name,
                ClassResult.FAILED,
                null,
                reason);

        }
        return new ClassReport(_testClass.Name,
            ClassResult.COMPLETED,
            reports);
    }

    private List<(string, TargetInvocationException)> RunMethods(MethodInfo[] methods, object? methodsObject)
    {
        var exceptions = new ConcurrentBag<(string, TargetInvocationException)>();
        Parallel.Invoke(methods
            .Select(method => new Action(() =>
            {
                try
                {
                    method.Invoke(methodsObject, null);
                }
                catch (TargetInvocationException e)
                {
                    exceptions.Add((method.Name, e));
                }
            }))
            .ToArray());
        return exceptions.ToList();
    }
}
