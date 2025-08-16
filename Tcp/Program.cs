// 1.Send message:
// printf "Do you have what it takes to be an engineer at TheStartup™?\r\n" | nc -w 1 127.0.0.1 40569

// 2. GET Request:
// curl http://localhost:40569/coffee
// Example in rawget.http

// 3. POST Request:
// curl -X POST -H "Content-Type: application/json" -d '{"flavor":"dark mode"}' http://localhost:42069/coffee
// Example in rawpost.http

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
        var lines = await GetLinesChannel(stream);

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

static async Task<IEnumerable<string>> GetLinesChannel(Stream stream)
{
    const int bufferSize = 8;
    byte[] buffer = new byte[bufferSize];

    List<string> lines = [];
    string message = string.Empty;

    while (true)
    {
        Array.Clear(buffer);

        int readBytes = await stream.ReadAsync(buffer, 0, bufferSize);
        if (readBytes == 0)
            break;

        string bufferText = Encoding.UTF8.GetString(buffer);
        var newLineIndex = bufferText.IndexOf('\n');

        if (newLineIndex > -1)
        {
            var endOfLineText = bufferText[..newLineIndex];
            message += endOfLineText;

            lines.Add(message);

            message = bufferText[(newLineIndex + 1)..];

            continue;
        }

        message += bufferText;
    }

    lines.Add(message);
    return lines.AsEnumerable();
}
