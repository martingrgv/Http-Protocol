namespace Tcp.Request;

public class RequestHandler : IRequestHandler
{
    public Task Handle(Request request)
    {
        Console.WriteLine("[SERVER] Received request:");
        Console.WriteLine($"[SERVER] Method: {request.RequestLine.Method}");
        Console.WriteLine($"[SERVER] Target: {request.RequestLine.RequestTarget}");
        Console.WriteLine($"[SERVER] Http Version: {request.RequestLine.HttpVersion}");

        return Task.CompletedTask;
    }
}
