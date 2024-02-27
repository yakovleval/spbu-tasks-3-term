using MyNUnit;
using System.Text.Json.Serialization;

namespace MyNUnitWeb.Server.Data;

public class ClassResult
{
    public int ClassResultId { get; set; }
    public string ClassName { get; set; }
    MyNUnit.ClassResult Status { get; set; }
    public string? Reason { get; private set; }
    public List<MethodResult>? MethodResults { get; set; }
    [JsonIgnore]
    public int AssemblyResultId { get; set; }

    public void Init(ClassReport classReport)
    {
        ClassName = classReport.className;
        Status = classReport.state;
        if (Status == MyNUnit.ClassResult.FAILED)
        {
            Reason = classReport.reason;
        }
        else
        {
            if (classReport.testReports == null)
            {
                return;
            }
            MethodResults = new();
            foreach (var testReport in classReport.testReports)
            {
                var methodResult = new MethodResult();
                methodResult.Init(testReport);
                MethodResults.Add(methodResult);
            }
        }
    }
}
