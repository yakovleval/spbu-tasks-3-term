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
using (client)
{
    while (true)
    {
        Console.WriteLine("Enter the command:");
        var request = Console.ReadLine();
        if (request is null || !Regex.IsMatch(request, @"[012]( \w)?"))
        {
            Console.WriteLine("incorrect command");
            continue;
        }
        if (request[0] == '0')
        {
            return;
        }
        try
        {
            Console.WriteLine(await client.SendRequestAsync(request));
        }
        catch
        {
            Console.WriteLine("unable to send a request");
        }
    }
}