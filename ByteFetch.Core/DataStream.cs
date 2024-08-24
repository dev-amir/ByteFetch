using ByteFetch.Shared;

namespace ByteFetch.Core;

internal class DataStream(InProgressDownloadModel inProgressDownloadModel, DownloadStatus downloadStatus, DownloadConfig config, FileSegmentWriter segmentWriter, CancellationTokenSource cts)
{
    private readonly DownloadStatus _downloadStatus = downloadStatus;
    private readonly InProgressDownloadModel _inProgressDownloadModel = inProgressDownloadModel;
    private readonly DownloadConfig _config = config;
    private readonly FileSegmentWriter _segmentWriter = segmentWriter;
    private readonly CancellationTokenSource _cts = cts;
    public async Task Start()
    {
        using var client = new HttpClient();
        var writeManagerTask = Task.Run(() => _segmentWriter.WriteManagerAsync());
        var streamTasks = new Task[_config.NumberOfThreads];
        try
        {
            for (var i = 0; i < _config.NumberOfThreads; i++)
            {
                var start = i * _config.SegmentSize;
                var end = _config.NumberOfThreads - 1 == i ? _inProgressDownloadModel.DownloadSize - 1 : start + _config.SegmentSize - 1;
                streamTasks[i] = StreamSegmentAsync(client, start, end);
            }
            await Task.WhenAll(streamTasks);
        }
        catch
        {
            if (!_cts.Token.IsCancellationRequested)
                _cts.Cancel();
        }
        await writeManagerTask;

    }

    private async Task StreamSegmentAsync(HttpClient client, long start, long end)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, _inProgressDownloadModel.URI.AbsoluteUri);
        request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(start, end);
        using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        using var responseStream = await response.Content.ReadAsStreamAsync();
        int bytesRead;
        var buffer = new byte[_config.BufferSize];
        while ((bytesRead = await responseStream.ReadAsync(buffer, _cts.Token)) > 0)
        {
            _segmentWriter.AddBuffer((byte[])buffer.Clone(), start, bytesRead);
            _inProgressDownloadModel.StreamedSize += bytesRead;
            start += bytesRead;
        }
    }
}
