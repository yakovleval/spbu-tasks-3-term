using System.Diagnostics;
using System.Text;
using Test1attempt1;

const string help = "usage: dotnet run <path>";
if (args.Length != 1 || !File.Exists(args[0]) && !Directory.Exists(args[0]))
{
    Console.WriteLine(help);
    return;
}

string path = args[0];

var watch = new Stopwatch();
watch.Start();
var result = CheckSum.Evaluate(path);
watch.Stop();

Console.WriteLine($"regular version: {watch.ElapsedMilliseconds} ms");
Console.WriteLine($"result: {Convert.ToHexString(result)}");

watch.Restart();
result = await CheckSum.EvaluateParallel(path);
watch.Stop();

Console.WriteLine($"paralleled version: {watch.ElapsedMilliseconds} ms");
Console.WriteLine($"result: {Convert.ToHexString(result)}");
