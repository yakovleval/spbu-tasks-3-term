using Microsoft.AspNetCore.Mvc;
using MyNUnitWeb.Server.Data;
using System.Reflection.Metadata.Ecma335;

namespace MyNUnitWeb.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RunTestsController : ControllerBase
    {
        private readonly HistoryDbContext _context;
        private readonly string _uploads = "";

        public RunTestsController(IWebHostEnvironment env, HistoryDbContext context)
        {
            _context = context;
            _uploads = "wwwroot";
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
                        string filePath = Path.Combine(_uploads, file.FileName);
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
                var dir = Directory.GetFiles(_uploads);
                var assemblies = new List<AssemblyResult>();
                int i = 0;
                foreach (var filePath in dir)
                {
                    if (filePath.Contains("MyNUnit.dll"))
                    {
                        continue;
                    }
                    var tester = new MyNUnit.MyNUnit(filePath);
                    var result = tester.RunTests();
                    var assembly = new AssemblyResult();
                    assembly.Init(result);
                    i++;
                    if (assembly.TestsNumber > 0)
                    {
                        var entity = _context.Assemblies.Add(assembly);
                        assembly.AssemblyResultId = entity.Entity.AssemblyResultId;
                        assemblies.Add(assembly);
                        foreach (var classResult in assembly.ClassResults)
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
                await _context.SaveChangesAsync();
                return Ok(assemblies);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }
    }
}
