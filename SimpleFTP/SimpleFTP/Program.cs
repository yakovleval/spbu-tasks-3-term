// See https://aka.ms/new-console-template for more information

Task<int> Async()
{
    //await Task.Delay(3000);
    Console.WriteLine("hello from async");
    return Task.FromResult(13);
}

Console.WriteLine("Hello from main");
await Async();
Console.WriteLine("additional work from main");
Thread.Sleep(4000);
