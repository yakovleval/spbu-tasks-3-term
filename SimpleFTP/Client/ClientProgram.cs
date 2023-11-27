using System.Net;
using System.Text.RegularExpressions;
string help = """
    usage: dotnet run <ip> <port>
    """;

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
Client.Client client;
try
{
    client = new(ip.ToString(), port);
}
catch
{
    Console.WriteLine("unable to connect to the server");
    return;
}
Console.WriteLine("type 0 to quit");
while (true)
{
    Console.WriteLine("Enter the command:");
    var request = Console.ReadLine();
    if (request is null)
    {
        return;
    }
    if (!Regex.IsMatch(request, @"[012]( [a-zA-Z_0-9.\/]+)?"))
    {
        Console.WriteLine("incorrect command");
        continue;
    }
    switch (request[0])
    {
        case '0':
            return;
        case '1':
            Console.WriteLine(await client.ListAsync(request));
            break;
        default:
            try
            {
                var path = request.Split(" ")[1];
                var name = Path.GetFileName(path);
                Console.WriteLine("enter the path of downloaded file:");
                var targetPath = Console.ReadLine();
                if (targetPath == null)
                {
                    break;
                }
                var file = await client.GetAsync(request);
                File.WriteAllBytes(Path.Combine(targetPath, name), file);
                break;
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("file not fould");
            }
            break;
    }
}