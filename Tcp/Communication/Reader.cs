using System.Text;

namespace Tcp.Communication;

public class Reader : IReader
{
    private const int BufferSize = 8;

    public async Task<IEnumerable<string>> ReadAsync(Stream stream)
    {
        byte[] buffer = new byte[BufferSize];

        List<string> lines = [];
        string message = string.Empty;

        while (true)
        {
            Array.Clear(buffer);

            int readBytes = await stream.ReadAsync(buffer, 0, BufferSize);
            if (readBytes == 0)
                break;

            string bufferText = Encoding.UTF8.GetString(buffer, 0, readBytes);

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

        if (!string.IsNullOrEmpty(message))
        {
            lines.Add(message);
        }

        return lines.AsEnumerable();
    }
}
