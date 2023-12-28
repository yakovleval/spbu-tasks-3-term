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
var myNUnit = new MyNUnit.MyNUnit(path);
myNUnit.RunTestsAndPrintResult();
