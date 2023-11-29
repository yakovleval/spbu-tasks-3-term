using System;
using System.Reflection;

const string help = "usage: dotnet run <path-to-assemblies>";
if (args.Length != 1)
{
    Console.WriteLine(help);
    return;
}
var path = args[0];
if (!Directory.Exists(path))
{
    Console.WriteLine("Directory doesn't exist");
    return;
}
//var ass = Assembly.LoadFrom("C:\\Users\\aleksandr\\source\\repos\\spbu-tasks-3-term\\MyNUnit\\TestProject\\release\\TestProject.dll")!;
//var type = ass.ExportedTypes.First();
//var instance = Activator.CreateInstance(type);
//var method = type.GetMethod("TestPasses")!;
//method.Invoke(instance, null);
var myNUnit = new MyNUnit.MyNUnit(path);
myNUnit.RunTestsAndPrintResult();
