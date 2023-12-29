using System.Text;

namespace MyNUnit;

public enum TestResult
{
    PASSED,
    FAILED,
    IGNORED
}

public enum ClassResult
{
    COMPLETED,
    IGNORED
}

/// <summary>
/// class which represents result of a test run
/// </summary>
/// <param name="methodName">name of the test method</param>
/// <param name="state">result of the run</param>
/// <param name="reason">reason for ignoring test if it was ignored</param>
/// <param name="timeElapsed">time elapsed during test run</param>
public record TestReport(string methodName, TestResult state, string? reason = null, long timeElapsed = 0)
{
    public override string ToString() => state switch
    {
        TestResult.PASSED => $"{methodName}() -- OK, time elapsed: {timeElapsed}\n",
        TestResult.FAILED => $"{methodName}() -- FAILED, reason: {reason}, time elapsed: {timeElapsed}\n",
        _ => $"{methodName}() -- IGNORED, reason: {reason}\n"
    };
}

/// <summary>
/// class which represents result of running all tests in a test class
/// </summary>
/// <param name="className">name of the test class</param>
/// <param name="state">result of the run</param>
/// <param name="testReports">results of all test methods in the class</param>
/// <param name="reason">reason for ignoring the class if it was ignored</param>
public record ClassReport(string className,
    ClassResult state,
    TestReport[]? testReports = null,
    string? reason = null)
{
    public override string ToString()
    {
        if (state == ClassResult.COMPLETED)
        {
            var result = new StringBuilder();
            result.Append($"{className}:\n");
            foreach (var report in testReports!)
            {
                result.Append(report);
            }
            return result.ToString();
        }
        return $"{className} -- IGNORED, reason: {reason}\n";
    }
}
