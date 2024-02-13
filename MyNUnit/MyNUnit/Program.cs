const string help = "usage: dotnet run <path-to-assemblies>";
if (args.Length != 1)
{
    Console.WriteLine(help);
    return;
}
var path = args[0];

var myNUnit = new MyNUnit.MyNUnit("C:\\Users\\aleksandr\\source\\repos\\spbu-tasks-3-term\\Kek\\Kek\\bin\\Debug\\net8.0\\Kek.dll");
myNUnit.RunTestsAndPrintResult();
