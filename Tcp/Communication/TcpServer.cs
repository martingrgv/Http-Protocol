using System.Net;
using System.Net.Sockets;
using Tcp.Request;

namespace Tcp.Communication;

public class TcpServer : IDisposable
{
    private const int PortMaxSize = 5;

    private readonly CancellationTokenSource _cts = new();

    private readonly TcpListener _tcpListener;
    private TcpClient? _tcpClient;

    private TcpServer(TcpListener listener)
    {
        _tcpListener = listener;
    }

    public static TcpServer OpenConnection(string address, int port)
    {
        ArgumentException.ThrowIfNullOrEmpty(address);
        ArgumentOutOfRangeException.ThrowIfNegative(port);
        ArgumentOutOfRangeException.ThrowIfNegative(port);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(port.ToString().Length, PortMaxSize);

        var listener = new TcpListener(IPAddress.Parse(address), port);
        listener.Start();

        Console.WriteLine($"[SERVER] Listening on {address}:{port}");

        return new TcpServer(listener);
    }

    public async Task ReceiveRequestAsync(IRequestHandler handler)
    {
        while (!_cts.Token.IsCancellationRequested)
        {
            try
            {
                _tcpClient = await _tcpListener.AcceptTcpClientAsync();
                Console.WriteLine("[SERVER] Client connected");

                NetworkStream stream = _tcpClient.GetStream();
                var request = await RequestParser.RequestFromStreamAsync(stream, _cts.Token);
                await handler.Handle(request);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SERVER] Error: {ex.Message}");
            }
        }
    }

    public async Task ReceiveMessageAsync(IReader reader, IMessageHandler handler)
    {
        while (!_cts.Token.IsCancellationRequested)
        {
            try
            {
                _tcpClient = await _tcpListener.AcceptTcpClientAsync();
                Console.WriteLine("[SERVER] Client connected");

                NetworkStream stream = _tcpClient.GetStream();
                var messages = await reader.ReadAsync(stream);
                await handler.Handle(messages);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SERVER] Error: {ex.Message}");
            }
        }
    }

    public void CloseConnection()
    {
        _cts.Cancel();
        _tcpListener.Stop();
    }

    public void Dispose()
    {
        CloseConnection();
    }
}
