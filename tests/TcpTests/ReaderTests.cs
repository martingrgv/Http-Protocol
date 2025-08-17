using System.Text;
using Tcp.Communication;

namespace TcpTests;

[TestFixture]
public class ReaderTests
{
    private Reader _reader;

    [SetUp]
    public void SetUp()
    {
        _reader = new Reader();
    }

    [Test]
    public async Task ReaderReturnsFullMessage_WhenHttpGetMessage()
    {
        string httpRequest = "GET /coffee HTTP/1.1\r\nHost: localhost:40569\r\nUser-Agent: curl/8.15.0\r\nAccept: */*\r\n\r\n";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(httpRequest));

        var result = await _reader.ReadAsync(stream);
        var lines = result.ToList();

        Console.WriteLine(string.Join("\n", lines));

        Assert.That(lines, Has.Count.EqualTo(5));
        Assert.That(lines[0], Is.EqualTo("GET /coffee HTTP/1.1\r"));
        Assert.That(lines[1], Is.EqualTo("Host: localhost:40569\r"));
        Assert.That(lines[2], Is.EqualTo("User-Agent: curl/8.15.0\r"));
        Assert.That(lines[3], Is.EqualTo("Accept: */*\r"));
        Assert.That(lines[4], Is.EqualTo("\r\n"));
    }

    [Test]
    public async Task ReaderReturnsFullMessageWithBody_WhenHttpPostMessage()
    {
        string httpRequest = "POST /coffee HTTP/1.1\r\nHost: localhost:40569\r\nUser-Agent: curl/8.15.0\r\nAccept: */*\r\nContent-Type: application/json\r\nContent-Length: 22\r\n\r\n{\"flavor\":\"dark mode\"}\r";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(httpRequest));

        var result = await _reader.ReadAsync(stream);
        var lines = result.ToList();

        Assert.That(lines, Has.Count.EqualTo(8));
        Assert.That(lines[0], Is.EqualTo("POST /coffee HTTP/1.1\r"));
        Assert.That(lines[1], Is.EqualTo("Host: localhost:40569\r"));
        Assert.That(lines[2], Is.EqualTo("User-Agent: curl/8.15.0\r"));
        Assert.That(lines[3], Is.EqualTo("Accept: */*\r"));
        Assert.That(lines[4], Is.EqualTo("Content-Type: application/json\r"));
        Assert.That(lines[5], Is.EqualTo("Content-Length: 22\r"));
        Assert.That(lines[6], Is.EqualTo("\r"));
        Assert.That(lines[7], Is.EqualTo("{\"flavor\":\"dark mode\"}\r"));
    }

    [Test]
    public async Task ReaderReturnsEmptyMessages_WhenStreamIsEmpty()
    {
        var stream = new MemoryStream();

        var lines = await _reader.ReadAsync(stream) as List<string>;

        Assert.That(lines, Has.Count.EqualTo(0));
    }
}
