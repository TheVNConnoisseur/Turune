using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Turune.Services;

public interface IFileLoader
{
    Task<byte[]> ReadBytesAsync(string path, CancellationToken ct = default);
    Task<string[]> ReadLinesAsync(string path, CancellationToken ct = default);
    Task WriteBytesAsync(string path, byte[] data, CancellationToken ct = default);
    Task WriteLinesAsync(string path, string[] lines, CancellationToken ct = default);
    Task<IReadOnlyList<byte[]>> ReadBytesManyAsync(IReadOnlyList<string> paths, CancellationToken ct = default);
}
public sealed class FileLoader : IFileLoader
{
    public Task<byte[]> ReadBytesAsync(string path, CancellationToken ct = default)
        => File.ReadAllBytesAsync(path, ct);

    public Task<string[]> ReadLinesAsync(string path, CancellationToken ct = default)
        => File.ReadAllLinesAsync(path, ct);

    public Task WriteBytesAsync(string path, byte[] data, CancellationToken ct = default)
        => File.WriteAllBytesAsync(path, data, ct);

    public Task WriteLinesAsync(string path, string[] lines, CancellationToken ct = default)
        => File.WriteAllLinesAsync(path, lines, ct);

    public async Task<IReadOnlyList<byte[]>> ReadBytesManyAsync(IReadOnlyList<string> paths, CancellationToken ct = default)
    {
        var result = new List<byte[]>(paths.Count);
        foreach (var p in paths)
            result.Add(await File.ReadAllBytesAsync(p, ct));
        return result;
    }
}