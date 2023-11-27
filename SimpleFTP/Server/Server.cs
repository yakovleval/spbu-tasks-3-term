using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace Server;

public class Server
{
    private readonly TcpListener _listener;

    public Server(IPAddress ip, int port)
    {
        _listener = new TcpListener(ip, port);
    }

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

    private static async Task ClientHanderAsync(TcpClient client)
    {
        using var stream = client.GetStream();
        using var reader = new StreamReader(client.GetStream());
        using var writer = new StreamWriter(stream) { AutoFlush = true };
        try
        {
            while (client.Connected)
            {
                var line = await reader.ReadLineAsync();
                if (line is null || !Regex.IsMatch(line, @"[12] [a-zA-Z_0-9.\/]+"))
                {
                    continue;
                }
                var request = line.Split(" ")[0];
                var path = line.Split(" ")[1];
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
        }
        finally
        {
            await Console.Out.WriteLineAsync("client disconnected");
            client.Dispose();
        }
    }

    private static async Task ListRequestHandlerAsync(string path, StreamWriter writer)
    {
        if (!Directory.Exists(path))
        {
            await writer.WriteLineAsync("-1");
            return;
        }
        var dirs = Directory.GetDirectories(path);
        var files = Directory.GetFiles(path);
        List<string> stringBuilder = new();
        stringBuilder.Add($"{dirs.Length + files.Length}");
        foreach (var dir in dirs)
        {
            stringBuilder.Add($"{Path.GetRelativePath(".", dir)} true");
        }
        foreach (var file in files)
        {
            stringBuilder.Add($"{Path.GetRelativePath(".", file)} false");
        }
        await writer.WriteLineAsync(String.Join(' ', stringBuilder));
    }

    private static async Task GetRequestHandlerAsync(string path, Stream stream)
    {
        if (!File.Exists(path))
        {
            await stream.WriteAsync(BitConverter.GetBytes(-1L));
            return;
        }
        var bytes = await File.ReadAllBytesAsync(path);
        var length = BitConverter.GetBytes((long)bytes.Length);
        var space = BitConverter.GetBytes(' ');

        await stream.WriteAsync(length);
        await stream.WriteAsync(space);
        await stream.WriteAsync(bytes);
    }
}
