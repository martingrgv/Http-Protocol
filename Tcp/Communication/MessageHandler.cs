namespace Tcp.Communication;

public class MessageHandler : IMessageHandler
{
    public Task Handle(IEnumerable<string> lines)
    {
        foreach (var line in lines)
        {
            Console.WriteLine($"[SERVER] Received: {line}");
        }

        return Task.CompletedTask;
    }
}
