using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

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
            await Console.Out.WriteLineAsync("client connected");
            _ = Task.Run(async () => await ClientHanderAsync(client));
        }
    }

    private async Task ClientHanderAsync(TcpClient client)
    {
        using var stream = client.GetStream();
        using var reader = new StreamReader(stream);
        using var writer = new StreamWriter(stream) { AutoFlush = true };
        using (client)
        {
            while (client.Connected)
            {
                var line = await reader.ReadLineAsync();
                if (line is null || !Regex.IsMatch(line, @"[12] [a-zA-Z_0-9.\/]+"))
                {
                    continue;
                }
                var (request, path) = (line.Split(" ")[0], line.Split(" ")[1]);
                if (!File.Exists(path) && !Directory.Exists(path))
                {
                    await writer.WriteLineAsync("-1");
                    continue;
                }
                switch (request)
                {
                    case "1":
                        await ListRequestHandlerAsync(path, writer);
                        break;
                    case "2":
                        await GetRequestHandlerAsync(path, stream);
                        break;
                    default:
                        await writer.WriteLineAsync("-1");
                        break;
                }
            }
            await Console.Out.WriteLineAsync("client disconnected");
        }
    }

    private async Task ListRequestHandlerAsync(string path, StreamWriter writer)
    {
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

    private async Task GetRequestHandlerAsync(string path, Stream stream)
    {
        var bytes = await File.ReadAllBytesAsync(path);
        var length = BitConverter.GetBytes(bytes.Length);
        var space = BitConverter.GetBytes(' ');

        await stream.WriteAsync(length);
        await stream.WriteAsync(space);
        await stream.WriteAsync(bytes);
    }
}
