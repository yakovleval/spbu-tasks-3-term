using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Client;

public class Client : IDisposable
{
    private readonly TcpClient _client;
    private readonly StreamReader _reader;
    private readonly StreamWriter _writer;

    public Client(string ip, int port)
    {
        _client = new TcpClient(ip, port);
        _reader = new StreamReader(_client.GetStream());
        _writer = new StreamWriter(_client.GetStream()) { AutoFlush = true };
    }

    public void Dispose()
    {
        _client.Close();
        _reader.Close();
        _writer.Close();
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
        byte[] firstNumber = new byte[sizeof(int)];
        await stream.ReadExactlyAsync(firstNumber, 0, sizeof(int));
        int size = BitConverter.ToInt32(firstNumber);
        if (size == -1)
        {
            throw new FileNotFoundException();
        }
        stream.ReadByte();
        stream.ReadByte();
        byte[] file = new byte[size];
        await stream.ReadExactlyAsync(file);
        return file;
    }
}
