using System.Text.RegularExpressions;

Console.WriteLine("enter ip address:");
var ip = Console.ReadLine();
if (ip is null)
{
    Console.WriteLine("invalid ip");
    return;
}
Console.WriteLine("enter port:");
int port;
if (!int.TryParse(Console.ReadLine(), out port) ||
    port < 1 ||
    port > 1 << 16)
{
    Console.WriteLine("invalid port");
    return;
}
Client.Client client;
try
{
    client = new(ip, port);
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
}