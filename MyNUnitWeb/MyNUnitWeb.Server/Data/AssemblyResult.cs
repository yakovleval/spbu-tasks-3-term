using MyNUnit;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNUnitWeb.Server.Data;

public class AssemblyResult
{
    public int AssemblyResultId { get; set; }
    public string AssemblyName { get; set; } = "";
    public List<ClassResult> ClassResults { get; set; } = new();
    public int Passed { get; private set; }
    public int Failed { get; private set; }
    public int Ignored { get; private set; }
    [NotMapped]
    public int TestsNumber => Passed + Failed + Ignored;
    public void Init(AssemblyReport assemblyReport)
    {
        AssemblyName = assemblyReport.assemblyName;
        foreach (var classReport in assemblyReport.classReports)
        {
            var classResult = new ClassResult();
            classResult.Init(classReport);
            ClassResults.Add(classResult);
        }
        foreach (var classResult in ClassResults)
        {
            if (classResult.MethodResults == null)
            {
                return;
            }
            foreach (var methodResult in classResult.MethodResults)
            {
                switch (methodResult.Status)
                {
                    case TestResult.PASSED:
                        Passed++;
                        break;
                    case TestResult.FAILED:
                        Failed++;
                        break;
                    case TestResult.IGNORED:
                        Ignored++;
                        break;
                }
            }
        }
    }
}
