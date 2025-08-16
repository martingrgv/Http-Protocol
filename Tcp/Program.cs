// Run in terminal to test:
// printf "Do you have what it takes to be an engineer at TheStartup™?\r\n" | nc -w 1 127.0.0.1 40569

using System.Net;
using System.Net.Sockets;
using System.Text;

var ipEndPoint = new IPEndPoint(IPAddress.Loopback, 40569);
var tcpListener = new TcpListener(ipEndPoint);

tcpListener.Start();
Console.WriteLine("[SERVER] Listening on 127.0.0.1:40569");

while (true)
{
    try
    {

        var tcpClient = await tcpListener.AcceptTcpClientAsync();
        Console.WriteLine("[SERVER] Client connected");

        NetworkStream stream = tcpClient.GetStream();
        var lines = GetLinesChannel(stream);

        foreach (var line in lines)
        {
            Console.WriteLine($"[SERVER] Recieved: {line}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERROR] {ex.Message}");
    }
}

static IEnumerable<string> GetLinesChannel(Stream stream)
{
    int bufferSize = 8;
    byte[] buffer = new byte[bufferSize];

    List<string> lines = [];
    var sb = new StringBuilder();

    while (stream.Read(buffer, 0, bufferSize) > 0)
    {
        string bufferText = Encoding.UTF8.GetString(buffer);
        var newLineIndex = bufferText.IndexOf('\n');

        if (newLineIndex > -1)
        {
            var endOfLineText = bufferText[..newLineIndex];
            sb.Append(endOfLineText);

            lines.Add(sb.ToString());

            sb.Clear();
            sb.Append(bufferText.AsSpan(newLineIndex + 1));

            continue;
        }

        sb.Append(bufferText);
    }

    return lines.AsEnumerable();
}
