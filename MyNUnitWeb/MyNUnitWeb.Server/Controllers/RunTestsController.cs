using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyNUnitWeb.Server.Data;
using System.Net.WebSockets;
using System.Runtime.InteropServices.Marshalling;
using System.Runtime.Loader;

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
        try
        {
            var loadContext = new AssemblyLoadContext("DisposableContext", true);
            var assemblies = new List<AssemblyResult>();
            int i = 0;
            foreach (var filePath in _uploads.GetFiles())
            {
                if (filePath.Name.Contains("MyNUnit.dll"))
                {
                    continue;
                }
                using var fs = new FileStream(filePath.FullName, FileMode.Open, FileAccess.Read);
                var assembly = loadContext.LoadFromStream(fs);
                var tester = new MyNUnit.MyNUnit(assembly);
                var assemblyReport = tester.RunTests();
                var assemblyResult = new AssemblyResult();
                assemblyResult.Init(assemblyReport);
                i++;
                if (assemblyResult.TestsNumber > 0)
                {
                    var entity = _context.Assemblies.Add(assemblyResult);
                    assemblyResult.AssemblyResultId = entity.Entity.AssemblyResultId;
                    assemblies.Add(assemblyResult);
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
            }
            loadContext.Unload();
            await _context.SaveChangesAsync();
            return Redirect("GetHistory");
        }
        catch (Exception e)
        {
            return StatusCode(500, e);
        }
        finally
        {
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
            foreach (var file in _uploads.GetFiles())
            {
                file.Delete();
            }
        }
    }
}
