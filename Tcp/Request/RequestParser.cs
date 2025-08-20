using System.Text;
using System.Text.RegularExpressions;

namespace Tcp.Request;

public static class RequestParser
{
    private const int BufferSize = 1024;

    private const int RequestMinLinesCount = 5;
    private const int RequestLineLength = 3;
    private static readonly Regex HttpVersionRegex = new Regex(
        @"^HTTP\/(?<version>\d+\.\d+)$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static Request RequestFromReader(IEnumerable<string> receivedLines)
    {
        var lines = receivedLines.ToArray();
        ArgumentOutOfRangeException.ThrowIfZero(lines.Length);
        ArgumentOutOfRangeException.ThrowIfLessThan(lines.Length, RequestMinLinesCount);

        var requestLine = RequestLineFromReader(lines);
        return new Request(requestLine);
    }

    public static async Task<Request> RequestFromStreamAsync(Stream stream, CancellationToken cancellationToken)
    {
        byte[] buffer = new byte[BufferSize];
        string requestMessage = string.Empty;
        Request? request = null;

        while (!cancellationToken.IsCancellationRequested ||
                request is null)
        {
            Array.Clear(buffer);
            int readBytes = await stream.ReadAsync(buffer, 0, BufferSize);
            if (readBytes == 0)
                break;

            string bufferText = Encoding.UTF8.GetString(buffer, 0, readBytes);
            requestMessage += bufferText;

            if (requestMessage.EndsWith("\r\n"))
            {
                var lines = requestMessage.Split("\r\n");
                var requestLine = RequestLineFromReader(lines);
                request = new Request(requestLine);
            }
        }

        return request;
    }

    private static RequestLine RequestLineFromReader(string[] lines)
    {
        string[] requestLineInfo = lines[0].Split();
        ArgumentOutOfRangeException.ThrowIfNotEqual(requestLineInfo.Length, RequestLineLength);

        string method = requestLineInfo[0];
        string target = requestLineInfo[1];
        Match versionMatch = HttpVersionRegex.Match(requestLineInfo[2]);

        if (!method.All(char.IsUpper))
            throw new ArgumentException($"Method {method} must contain only capital letters!");
        if (!target.StartsWith('/'))
            throw new ArgumentException($"Request Target {target} must start with '/'!");
        if (!versionMatch.Success)
            throw new ArgumentException($"Http Version {versionMatch.Value} must be a valid version!");

        return new RequestLine(
                Method: method,
                RequestTarget: target,
                HttpVersion: versionMatch.Groups["version"].Value);
    }
}
