using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Server;

/// <summary>
/// represents a simple FTP server 
/// </summary>
public class Server
{
    private readonly TcpListener _listener;
    private readonly CancellationTokenSource _source = new();

    /// <summary>
    /// initializes a new instance of Server class that listens to the specified ip address and port
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    public Server(IPAddress ip, int port)
    {
        _listener = new TcpListener(ip, port);
    }

    /// <summary>
    /// asynchronously starts the server
    /// </summary>
    /// <returns></returns>
    public async Task StartAsync()
    {
        try
        {
            _listener.Start();
            await Console.Out.WriteLineAsync($"working on {_listener.LocalEndpoint}");
            while (true)
            {
                var client = await _listener.AcceptTcpClientAsync(_source.Token);
                await Console.Out.WriteLineAsync("client connected");
                _ = Task.Run(async () => await ClientHanderAsync(client));
            }
        }
        finally
        {
            _listener.Stop();
            await Console.Out.WriteLineAsync("server stopped");
        }
    }

    /// <summary>
    /// stops the server
    /// </summary>
    public void Stop()
    {
        _source.Cancel();
    }

    private async Task ClientHanderAsync(TcpClient client)
    {
        using var stream = client.GetStream();
        using var reader = new StreamReader(client.GetStream());
        using var writer = new StreamWriter(stream) { AutoFlush = true };
        try
        {
            while (client.Connected)
            {
                var line = await reader.ReadLineAsync(_source.Token);
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
        catch
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
            stringBuilder.Add($"{dir.Replace("\\", "/")} true");
        }
        foreach (var file in files)
        {
            stringBuilder.Add($"{file.Replace("\\", "/")} false");
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
