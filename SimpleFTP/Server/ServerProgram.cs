using System.Net;
using System.Net.Sockets;

int port = 8888;
var listener = new TcpListener(IPAddress.Any, port);
listener.Start();
while (true)
{
    var client = await listener.AcceptTcpClientAsync();
    Writer(client.GetStream());
    Reader(client.GetStream());
}

static void Writer(NetworkStream stream)
{
    Task.Run(() =>
    {
        var writer = new StreamWriter(stream) { AutoFlush = true };
        while (true)
        {
            var data = Console.ReadLine();
            Console.WriteLine("you: " + data);
            writer.Write(data + "\n");
        }
    });
}

static void Reader(NetworkStream stream)
{
    Task.Run(() =>
    {
        var reader = new StreamReader(stream);
        while (true)
        {
            var line = reader.ReadLine();
            Console.WriteLine("Client: " + line);
        }
    });
}