namespace Tcp.Communication;

public interface IMessageHandler
{
    Task Handle(IEnumerable<string> lines);
}
