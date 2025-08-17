namespace Tcp.Request;

public record RequestLine(string Method, string HttpVersion, string RequestTarget);
public record Request(RequestLine RequestLine);
