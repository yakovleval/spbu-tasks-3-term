using System.Net;

string help = "usage: dotnet run <ip> <port>";
IPAddress? ip;
int port;
if (args.Length != 2 ||
    !IPAddress.TryParse(args[0], out ip) ||
    ip is null ||
    !int.TryParse(args[1], out port))
{
    Console.WriteLine(help);
    return;
}

try
{
    Server.Server server = new(ip, port);
    await server.StartAsync();
}
catch
{
    Console.WriteLine("invalid port or ip");
}
