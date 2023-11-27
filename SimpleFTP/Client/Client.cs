using System.Net;
using System.Net.Sockets;

namespace Client;

/// <summary>
/// represents a simple FTP client
/// </summary>
public class Client : IDisposable
{
    private readonly TcpClient _client;
    private readonly StreamReader _reader;
    private readonly StreamWriter _writer;

    /// <summary>
    /// initializes a new instance of Client that connects to the specified ip address and port
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    public Client(IPAddress ip, int port)
    {
        _client = new TcpClient(ip.ToString(), port);
        _reader = new StreamReader(_client.GetStream());
        _writer = new StreamWriter(_client.GetStream()) { AutoFlush = true };
    }

    /// <summary>
    /// <inheritdoc cref="IDisposable.Dispose"/>
    /// </summary>
    public void Dispose()
    {
        _client.Dispose();
        _reader.Dispose();
        _writer.Dispose();
    }

    /// <summary>
    /// sends List request to the server
    /// </summary>
    /// <param name="path">path to directory to list</param>
    /// <returns>asynchronous Task with server response</returns>
    public async Task<string?> ListAsync(string path)
    {
        await _writer.WriteLineAsync("1 " + path);
        var response = await _reader.ReadLineAsync();
        return response == "-1" ? "directory not found" : response;
    }

    /// <summary>
    /// sends List request to the server
    /// </summary>
    /// <param name="path">path to file to get</param>
    /// <returns>asynchronous Task with server response</returns>
    /// <exception cref="FileNotFoundException">thrown if file is not found on the server</exception>
    /// <exception cref="InvalidDataException">thrown if server response is corrupted</exception>
    public async Task<byte[]> GetAsync(string path)
    {
        await _writer.WriteLineAsync("2 " + path);
        var stream = _client.GetStream();
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

    private static async Task<long> ReadLong(Stream stream)
    {
        byte[] array = new byte[sizeof(long)];
        int read = await stream.ReadAsync(array);
        if (read < array.Length)
        {
            throw new InvalidDataException();
        }
        return BitConverter.ToInt64(array);

    }

    private static async Task<char> ReadChar(Stream stream)
    {
        byte[] array = new byte[sizeof(char)];
        int read = await stream.ReadAsync(array);
        if (read < array.Length)
        {
            throw new InvalidDataException();
        }
        return BitConverter.ToChar(array);
    }
}
