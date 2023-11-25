using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;

const int port = 8888;
using (var client = new TcpClient("localhost", port))
{
    Writer(client.GetStream());
    Reader(client.GetStream());
    while (true) { }
}

static void Writer(NetworkStream stream)
{
    Task.Run(async () =>
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
            Console.WriteLine("Server: " + line);
        }
    });
}
