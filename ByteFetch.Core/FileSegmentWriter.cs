using ByteFetch.Shared;
using System.Collections.Concurrent;

namespace ByteFetch.Core;

internal class FileSegmentWriter(DownloadModel downloadModel, string pathAndFileName, CancellationTokenSource cts)
{
    private readonly ConcurrentQueue<(byte[] buffer, long start, int length)> _bufferQueue = new();
    private readonly string _pathAndFileName = pathAndFileName;
    private readonly DownloadModel _downloadModel = downloadModel;
    private readonly CancellationTokenSource _cts = cts;

    public void AddBuffer(byte[] buffer, long start, int length)
        => _bufferQueue.Enqueue((buffer, start, length));

    public async Task WriteManagerAsync()
    {
        using var fileStream = new FileStream(_pathAndFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        while (_downloadModel.DownloadSize > _downloadModel.StreamedSize && !_cts.Token.IsCancellationRequested)
        {
            await ProcessAndWrite(fileStream);
            await Task.Delay(500);
        }
        await ProcessAndWrite(fileStream);
    }

    private async Task ProcessAndWrite(FileStream fileStream)
    {
        while (_bufferQueue.TryDequeue(out var qItem))
        {
            fileStream.Seek(qItem.start, SeekOrigin.Begin);
            await fileStream.WriteAsync(qItem.buffer.AsMemory(0, qItem.length));
            await fileStream.FlushAsync();
            _downloadModel.WritedSize += qItem.length;
        }
    }
}