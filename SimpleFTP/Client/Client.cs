using System.Net.Sockets;

namespace Client;

public class Client : IDisposable
{
    private readonly TcpClient _client;
    private readonly StreamReader _reader;
    private readonly StreamWriter _writer;
    private readonly string _ip;
    private readonly int _port;

    public Client(string ip, int port)
    {
        _ip = ip;
        _port = port;
        _client = new TcpClient(ip, port);
        _reader = new StreamReader(_client.GetStream());
        _writer = new StreamWriter(_client.GetStream()) { AutoFlush = true };
    }

    public void Dispose()
    {
        _client.Dispose();
        _reader.Dispose();
        _writer.Dispose();
    }

    public async Task<string?> ListAsync(string request)
    {
        await _writer.WriteLineAsync(request);
        var response = await _reader.ReadLineAsync();
        return response == "-1" ? "directory not found" : response;
    }

    public async Task<byte[]> GetAsync(string request)
    {
        await _writer.WriteLineAsync(request);
        using var stream = _client.GetStream();
        long size = await ReadLong(stream);
        if (size == -1)
        {
            throw new FileNotFoundException();
        }
        await ReadChar(stream);
        byte[] file = new byte[size];
        int read = await stream.ReadAsync(file);
        if (read < file.Length)
        {
            throw new InvalidDataException();
        }
        return file;
    }

    private async Task<long> ReadLong(Stream stream)
    {
        byte[] array = new byte[sizeof(long)];
        int read = await stream.ReadAsync(array, 0, array.Length);
        if (read < array.Length)
        {
            throw new InvalidDataException();
        }
        return BitConverter.ToInt64(array);

    }

    private async Task<char> ReadChar(Stream stream)
    {
        byte[] array = new byte[sizeof(char)];
        int read = await stream.ReadAsync(array, 0, array.Length);
        if (read < array.Length)
        {
            throw new InvalidDataException();
        }
        return BitConverter.ToChar(array);
    }
}
