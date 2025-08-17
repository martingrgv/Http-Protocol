
namespace Tcp.Communication;

public class MessageHandler : IMessageHandler
{
    public Task Handle(IEnumerable<string> messages)
    {
        foreach (var message in messages)
        {
            Console.WriteLine($"[SERVER] Recieved: {message}");
        }

        return Task.CompletedTask;
    }
}
