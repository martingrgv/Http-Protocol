using Tcp.Communication;

var reader = new Reader();
var messageHandler = new MessageHandler();

using var tcpServer = TcpServer.OpenConnection("127.0.0.1", 40569);
await tcpServer.ReceiveAsync(reader, messageHandler);
