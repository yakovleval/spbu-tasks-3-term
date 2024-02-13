using Microsoft.AspNetCore.Mvc;
using MyNUnit;
using MyNUnitWeb.Server.Data;
using System;
using System.IO;
using System.Reflection;

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
            //_uploads = env.WebRootPath;
            _uploads = "C:\\Users\\aleksandr\\source\\repos\\spbu-tasks-3-term\\MyNUnitWeb\\MyNUnitWeb.Server\\wwwroot";
        }

        [HttpGet]
        [Route("GetHistory")]
        public IEnumerable<AssemblyResult>GetHistory()
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
            return assemblies;
        }

        [HttpPost]
        [Route("Upload")]
        public IActionResult Upload(IEnumerable<IFormFile> files)
        {
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    string filePath = Path.Combine(_uploads, file.FileName);
                    using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                }
            }
            return Ok();
        }
        [HttpGet]
        [Route("GetTestResults")]
        public async Task<IEnumerable<AssemblyResult>> TestResults()
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
                //assembly.AssemblyId = i;
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
            return assemblies;
        }
    }
}
