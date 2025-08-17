namespace Tcp.Communication;

public interface IReader
{
    Task<IEnumerable<string>> ReadAsync(Stream stream);
}
