namespace Tcp.Request;

public interface IRequestHandler
{
    Task Handle(Request request);
}

