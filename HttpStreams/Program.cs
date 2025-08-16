using System.Text;

using var fileStream = new FileStream("message.txt", FileMode.Open);
var lines = GetLinesChannel(fileStream);

foreach (var line in lines)
{
    Console.WriteLine($"read: {line}");
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
