using System.Net.Sockets;
using System.Net;

namespace Client;

public class Client
{
    private static readonly string IP = "localhost";
    private static readonly int PORT = 8888;
    private readonly TcpClient _client = new(IP, PORT);
    private readonly NetworkStream stream;
    private readonly StreamReader reader;
    private readonly StreamWriter writer;

    public Client()
    {
        stream = _client.GetStream();
        reader = new StreamReader(stream);
        writer = new StreamWriter(stream) { AutoFlush = true };

    }

    public async Task StartAsync()
    {
        while (true)
        {
            var request = await Console.In.ReadLineAsync();
            if (request is null)
            {
                await Console.Out.WriteLineAsync("error: empty respones");
                continue;
            }
            var response = await SendRequestAsync(request);
            await Console.Out.WriteLineAsync(response);
        }
    }

    private async Task<string?> SendRequestAsync(string request)
    {
        await writer.WriteLineAsync(request);
        return await reader.ReadLineAsync();
    }
}
