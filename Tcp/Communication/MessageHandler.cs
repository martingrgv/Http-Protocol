
using Tcp.Request;

namespace Tcp.Communication;

public class MessageHandler : IMessageHandler
{
    public Task Handle(IEnumerable<string> lines)
    {
        var request = RequestParser.RequestFromReader(lines);

        Console.WriteLine("[SERVER] Received request:");
        Console.WriteLine($"[SERVER] Method: {request.RequestLine.Method}");
        Console.WriteLine($"[SERVER] Target: {request.RequestLine.RequestTarget}");
        Console.WriteLine($"[SERVER] Http Version: {request.RequestLine.HttpVersion}");

        return Task.CompletedTask;
    }
}
