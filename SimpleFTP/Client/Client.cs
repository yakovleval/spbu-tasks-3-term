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

    public async Task<string?> SendRequestAsync(string request)
    {
        await _writer.WriteLineAsync(request);
        var response = await _reader.ReadLineAsync();
        return response == "-1" ? "file not found" : response;
    }
}
