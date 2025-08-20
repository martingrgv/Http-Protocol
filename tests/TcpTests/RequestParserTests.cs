using System.Text;
using Tcp.Request;

namespace TcpTests;

[TestFixture]
public class RequestParserTests
{
    [Test]
    public async Task ReturnsRequest_WhenFullStream()
    {
        string httpRequest = "GET /coffee HTTP/1.1\r\nHost: localhost:40569\r\nUser-Agent: curl/8.15.0\r\nAccept: */*\r\n\r\n";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(httpRequest));

        var request = await RequestParser.RequestFromStreamAsync(stream, CancellationToken.None);

        Assert.That(request, Is.Not.Null);
        Assert.That(request.RequestLine, Is.Not.Null);
        Assert.That(request.RequestLine.Method, Is.EqualTo("GET"));
        Assert.That(request.RequestLine.RequestTarget, Is.EqualTo("/coffee"));
        Assert.That(request.RequestLine.HttpVersion, Is.EqualTo("1.1"));
    }

    [Test]
    public void ReturnsGetRequest_WhenValidHttpGetLines()
    {
        var lines = new List<string>
        {
            "GET /coffee HTTP/1.1",
            "Host: localhost:40569",
            "User-Agent: curl/8.15.0",
            "Accept: */*",
            "\r\n"
        };

        var request = RequestParser.RequestFromReader(lines);

        Assert.That(request, Is.Not.Null);
        Assert.That(request.RequestLine, Is.Not.Null);
        Assert.That(request.RequestLine.Method, Is.EqualTo("GET"));
        Assert.That(request.RequestLine.RequestTarget, Is.EqualTo("/coffee"));
        Assert.That(request.RequestLine.HttpVersion, Is.EqualTo("1.1"));
    }

    [Test]
    public void ReturnsPostRequest_WhenValidPostLines()
    {
        var lines = new List<string>
        {
            "POST /coffee HTTP/1.1",
            "Host: localhost:40569",
            "User-Agent: curl/8.15.0",
            "Accept: */*",
            "Content-Type: application/json",
            "Content-Length: 22",
            "\r\n",
            "{\"flavor\":\"dark mode\"}"
        };

        var request = RequestParser.RequestFromReader(lines);

        Assert.That(request, Is.Not.Null);
        Assert.That(request.RequestLine, Is.Not.Null);
        Assert.That(request.RequestLine.Method, Is.EqualTo("POST"));
        Assert.That(request.RequestLine.RequestTarget, Is.EqualTo("/coffee"));
        Assert.That(request.RequestLine.HttpVersion, Is.EqualTo("1.1"));
    }

    [Test]
    public void ThrowsException_WhenRequestLineHasLength_LessThanThree()
    {
        var lines = new List<string>
        {
            "GET /coffee",
            "Host: localhost:40569",
            "User-Agent: curl/8.15.0",
            "Accept: */*",
            "\r\n"
        };
        Assert.Throws<ArgumentOutOfRangeException>(() => RequestParser.RequestFromReader(lines));
    }

    [Test]
    public void ThrowsException_WhenRequestLineHasLength_MoreThanThree()
    {
        var lines = new List<string>
        {
            "GET /coffee HTTP/1.1 Application",
            "Host: localhost:40569",
            "User-Agent: curl/8.15.0",
            "Accept: */*",
            "\r\n"
        };
        Assert.Throws<ArgumentOutOfRangeException>(() => RequestParser.RequestFromReader(lines));
    }

    [Test]
    public void ThrowsException_WhenMethodHasLowercaseLetters()
    {
        var lines = new List<string>
        {
            "Get /coffee HTTP/1.1",
            "Host: localhost:40569",
            "User-Agent: curl/8.15.0",
            "Accept: */*",
            "\r\n"
        };
        Assert.Throws<ArgumentException>(() => RequestParser.RequestFromReader(lines));
    }

    [Test]
    public void ThrowsException_WhenMethodHasDigits()
    {
        var lines = new List<string>
        {
            "GE1 /coffee HTTP/1.1",
            "Host: localhost:40569",
            "User-Agent: curl/8.15.0",
            "Accept: */*",
            "\r\n"
        };
        Assert.Throws<ArgumentException>(() => RequestParser.RequestFromReader(lines));
    }

    [Test]
    public void ThrowsException_WhenRouteTarget_DoesntStartWithSlash()
    {
        var lines = new List<string>
        {
            "GET coffee HTTP/1.1",
            "Host: localhost:40569",
            "User-Agent: curl/8.15.0",
            "Accept: */*",
            "\r\n"
        };
        Assert.Throws<ArgumentException>(() => RequestParser.RequestFromReader(lines));
    }

    [Test]
    public void ThrowsException_WhenHttpVersion_IsNotValid()
    {
        var lines = new List<string>
        {
            "GET coffee HTTP/1",
            "Host: localhost:40569",
            "User-Agent: curl/8.15.0",
            "Accept: */*",
            "\r\n"
        };
        Assert.Throws<ArgumentException>(() => RequestParser.RequestFromReader(lines));
    }

    [Test]
    public void ThrowsException_WhenEmptyLines()
    {
        var lines = new List<string>();
        Assert.Throws<ArgumentOutOfRangeException>(() => RequestParser.RequestFromReader(lines));
    }
}
