using Tcp.Communication;
using Tcp.Request;

var handler = new RequestHandler();

using var tcpServer = TcpServer.OpenConnection("127.0.0.1", 40569);
await tcpServer.ReceiveRequestAsync(handler);

