using MyNUnit;
using System.Text.Json.Serialization;

namespace MyNUnitWeb.Server.Data;

public class MethodResult
{
    public void Init(TestReport testReport)
    {
        MethodName = testReport.methodName;
        Status = testReport.state;
        Reason = testReport.reason;
        Expected = testReport.expected?.ToString();
        Was = testReport.was?.ToString();
        TimeElapsed = testReport.timeElapsed;
    }

    public int MethodResultId { get; set; }
    public string MethodName { get; set; }
    public TestResult Status { get; set; }
    public string? Reason { get; set; }

    public string? Expected { get; set; }
    public string? Was { get; set; }
    public long TimeElapsed { get; set; }
    [JsonIgnore]
    public int ClassResultId { get; set; }
}
