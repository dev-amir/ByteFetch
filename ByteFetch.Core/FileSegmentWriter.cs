using ByteFetch.Shared;
using System.Collections.Concurrent;

namespace ByteFetch.Core;

internal class FileSegmentWriter
{
    private readonly List<LoadedDataModel> _loadedBufferList;
    private readonly ConcurrentQueue<(byte[] buffer, long start)> _mergedBufferResultQueue = new();
    private readonly string _pathAndFileName;
    private readonly InProgressDownloadModel _inProgressDownloadModel;
    private readonly CancellationTokenSource _cts;
    private long _loadedSize = 0;

    public FileSegmentWriter(InProgressDownloadModel inProgressDownloadModel, string pathAndFileName, CancellationTokenSource cts)
    {
        _pathAndFileName = pathAndFileName;
        _inProgressDownloadModel = inProgressDownloadModel;
        _cts = cts;
        _loadedBufferList = new(_inProgressDownloadModel.NumberOfThreads);
        for (int i = 0; i < _inProgressDownloadModel.NumberOfThreads; i++)
            _loadedBufferList.Add(new LoadedDataModel());
    }

    public void AddBuffer(byte[] buffer, long start, int length, int index)
    {
        _loadedBufferList[index].Enqueue(buffer, start, length);
        _loadedSize += length;
    }
        

    public async Task WriteManagerAsync()
    {
        using var fileStream = new FileStream(_pathAndFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        while (_inProgressDownloadModel.DownloadSize > _inProgressDownloadModel.StreamedSize && !_cts.Token.IsCancellationRequested)
        {
            if (_loadedSize > _inProgressDownloadModel.MinWriteSize)
            {
                MergeBuffers();
                await WriteBuffers(fileStream);
            }
            await Task.Delay(500);
        }
        MergeBuffers();
        await WriteBuffers(fileStream);
    }

    private void MergeBuffers()
    {
        _loadedSize = 0;
        for (int i = 0; i < _inProgressDownloadModel.NumberOfThreads; i++)
        {
            var isMerged = _loadedBufferList[i].TryMerge(out var result);
            if (!isMerged)
                continue;
            _mergedBufferResultQueue.Enqueue(result);
        }
    }

    private async Task WriteBuffers(FileStream fileStream)
    {
        while(_mergedBufferResultQueue.TryDequeue(out var result))
        {
            fileStream.Seek(result.start, SeekOrigin.Begin);
            await fileStream.WriteAsync(result.buffer);
            _inProgressDownloadModel.WritedSize += result.buffer.Length;
        }
        await fileStream.FlushAsync();
    }
}

internal class LoadedDataModel
{
    private readonly ConcurrentQueue<(byte[] buffer, long start, int length)> _queue = new();
    private readonly object _lock = new object();
    private long _loadedLength;
    private long LoadedLength
    {
        get
        {
            lock(_lock)
            {
                var value = _loadedLength;
                _loadedLength = 0;
                return value;
            }
        }
        set
        {
            lock (_lock)
            {
                _loadedLength = value;
            }
        }
    }

    public void Enqueue(byte[] buffer, long start, int length)
    {
        _queue.Enqueue((buffer, start, length));
        LoadedLength += length;
    }

    public bool TryMerge(out (byte[] buffer, long start) result)
    {
        if (_queue.IsEmpty)
        {
            result = default;
            return false;
        }

        var totalLength = LoadedLength;
        var buffer = new byte[totalLength];
        int offset = 0;
        _queue.TryDequeue(out var firstItem);
        Array.Copy(firstItem.buffer, 0, buffer, offset, firstItem.length);
        offset += firstItem.length;
        totalLength -= firstItem.length;
        while (totalLength > 0)
        {
            _queue.TryDequeue(out var qItem);
            Array.Copy(qItem.buffer, 0, buffer, offset, qItem.length);
            offset += qItem.length;
            totalLength -= qItem.length;
        }
        result = (buffer, firstItem.start);
        return true;
    }
}
