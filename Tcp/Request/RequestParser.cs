using System.Text.RegularExpressions;

namespace Tcp.Request;

public static class RequestParser
{
    private const int RequestMinLinesCount = 5;
    private const int RequestLineLength = 3;
    private static readonly Regex HttpVersionRegex = new Regex(
        @"^HTTP/\d+\.\d+$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static Request RequestFromReader(IEnumerable<string> receivedLines)
    {
        var lines = receivedLines.ToArray();
        ArgumentOutOfRangeException.ThrowIfZero(lines.Length);
        ArgumentOutOfRangeException.ThrowIfLessThan(lines.Length, RequestMinLinesCount);

        var requestLine = RequestLineFromReader(lines);
        return new Request(requestLine);
    }

    private static RequestLine RequestLineFromReader(string[] lines)
    {
        string[] requestLineInfo = lines[0].Split();
        ArgumentOutOfRangeException.ThrowIfNotEqual(requestLineInfo.Length, RequestLineLength);

        string method = requestLineInfo[0];
        string target = requestLineInfo[1];
        string version = requestLineInfo[2];

        if (!method.All(char.IsUpper))
            throw new ArgumentException($"Method {method} must contain only capital letters!");
        if (!target.StartsWith('/'))
            throw new ArgumentException($"Request Target {target} must start with '/'!");
        if (!HttpVersionRegex.IsMatch(version))
            throw new ArgumentException($"Http Version {version} must be a valid version!");

        return new RequestLine(Method: method, RequestTarget: target, HttpVersion: version);
    }
}
