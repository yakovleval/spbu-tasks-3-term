using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server;

public class Server
{
    private static readonly string IP = "localhost";
    private static readonly int PORT = 8888;
    private readonly TcpListener _listener = new(IPAddress.Any, PORT);

    public async Task StartAsync()
    {
        _listener.Start();
        await Console.Out.WriteLineAsync($"working on {_listener.LocalEndpoint}");
        while (true)
        {
            var client = await _listener.AcceptTcpClientAsync();
            await Console.Out.WriteLineAsync("new client");
            ClientHander(client.GetStream());
        }
    }

    private void ClientHander(NetworkStream stream)
    {
        Task.Run(async () =>
        {
            using var reader = new StreamReader(stream);
            using var writer = new StreamWriter(stream) { AutoFlush = true };
            var line = await reader.ReadLineAsync();
            var splittedLine = line?.Split(" ");
            if (splittedLine is null || splittedLine.Length != 2)
            {
                await writer.WriteLineAsync("incorrect request");
                return;
            }
            var (request, path) = (splittedLine[0], splittedLine[1]);
            switch (request)
            {
                case "1":
                    ListRequestHandlerAsync(path, writer);
                    break;
                case "2":
                    GetRequestHandlerAsync(path, writer);
                    break;
                default:
                    await writer.WriteLineAsync("unknown request");
                    break;
            }
        });
    }

    private async void ListRequestHandlerAsync(string path, StreamWriter writer)
    {
        if (!Directory.Exists(path))
        {
            await writer.WriteLineAsync("-1");
            return;
        }
        var dirs = Directory.GetDirectories(path);
        var files = Directory.GetFiles(path);
        StringBuilder stringBuilder = new();
        stringBuilder.Append($"{dirs.Length + files.Length} ");
        foreach (var dir in dirs)
        {
            stringBuilder.Append($"{dir} true ");
        }
        foreach (var file in files)
        {
            stringBuilder.Append($"{file} false ");
        }
        await writer.WriteLineAsync(stringBuilder.ToString());
    }

    private async void GetRequestHandlerAsync(string path, StreamWriter writer)
    {
        if (!File.Exists(path))
        {
            await writer.WriteLineAsync("-1");
            return;
        }
        var bytes = File.ReadAllBytes(path);
        await writer.WriteLineAsync($"{bytes.Length} {bytes}");
    }
}
