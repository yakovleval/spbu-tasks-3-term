using Microsoft.AspNetCore.Mvc;
using MyNUnitWeb.Server.Data;

namespace MyNUnitWeb.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class RunTestsController : ControllerBase
{
    private readonly HistoryDbContext _context;
    private readonly DirectoryInfo _uploads;

    public RunTestsController(IWebHostEnvironment env, HistoryDbContext context)
    {
        _context = context;
        _uploads = new DirectoryInfo("wwwroot");
    }

    [HttpGet]
    [Route("GetHistory")]
    public IActionResult GetHistory()
    {
        try
        {
            var assemblies = _context.Assemblies.ToList();
            foreach (var assembly in assemblies)
            {
                var classes = _context.Classes.ToList().FindAll(classResult =>
                    classResult.AssemblyResultId == assembly.AssemblyResultId);
                foreach (var _class in classes)
                {
                    var methods = _context.Methods.ToList().FindAll(methodResult =>
                    methodResult.ClassResultId == _class.ClassResultId);
                    _class.MethodResults = methods;
                }
                assembly.ClassResults = classes;
            }
            assemblies.Reverse();
            return Ok(assemblies);
        }
        catch (Exception e)
        {
            return StatusCode(500, e);
        }
    }

    [HttpPost]
    [Route("Upload")]
    public IActionResult Upload(IEnumerable<IFormFile> files)
    {
        try
        {
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    string filePath = Path.Combine(_uploads.Name, file.FileName);
                    using var fileStream = new FileStream(filePath, FileMode.Create);
                    file.CopyTo(fileStream);
                }
            }
            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(500, e);
        }
    }

    [HttpGet]
    [Route("GetTestResults")]
    public async Task<IActionResult> TestResults()
    {
        var tester = new MyNUnit.MyNUnit(_uploads.FullName);
        var assemblyReports = tester.RunTests();
        var assemblyResults = assemblyReports
            .Select(report => new AssemblyResult(report))
            .ToArray();
        foreach (var assemblyResult in assemblyResults)
        {
            if (assemblyResult.ClassResults.Count > 0)
            {
                _context.Assemblies.Add(assemblyResult);
            }
            foreach (var classResult in assemblyResult.ClassResults)
            {
                _context.Classes.Add(classResult);
                if (classResult.MethodResults == null)
                    continue;
                foreach (var methodResult in classResult.MethodResults)
                {
                    _context.Methods.Add(methodResult);
                }
            }
        }
        tester = null;
        foreach (var file in _uploads.GetFiles())
        {
            file.Delete();
        }
        await _context.SaveChangesAsync();
        return Redirect("GetHistory");
    }
}
