using System.Reflection.Metadata.Ecma335;

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

public record TestReport(string methodName, TestResult state, string? reason = null, long timeElapsed = 0)
{
    public override string ToString()
    {
        return state switch
        {
            TestResult.PASSED => $"{methodName}() -- OK, time elapsed: {timeElapsed}\n",
            TestResult.FAILED => $"{methodName}() -- FAILED, reason: {reason}, time elapsed: {timeElapsed}\n",
            _ => $"{methodName}() -- IGNORED, reason: {reason}\n"
        };
    }
}

public record ClassReport(string className, 
    ClassResult state,
    TestReport[]? testReports = null, 
    string? reason = null)
{
    public override string ToString()
    {
        if (state == ClassResult.COMPLETED)
        {
            string result = $"{className}:\n";
            foreach (var report in testReports!)
            {
                result += report;
            }
            return result;
        }
        return $"{className} -- IGNORED, reason: {reason}\n";
    }
}
