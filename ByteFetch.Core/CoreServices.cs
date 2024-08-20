using ByteFetch.Shared;
using MimeTypes;

namespace ByteFetch.Core;

public class CoreServices
{
    private readonly CancellationTokenSource _cts = new();
    public async Task StartDownload(DownloadModel downloadModel, DownloadStatus downloadStatus)
    {
        var info = new DownloadInfo(downloadStatus, downloadModel.URI.AbsoluteUri);
        var config = new DownloadConfig(downloadModel);

        await info.GetHeaders();
        if (downloadStatus.IsRequestHeadersFailed)
            return;

        info.ProcessHeaders(downloadModel);
        config.CalculateSegmentsSizes(downloadModel);
        downloadModel.Name = GenerateFileName(downloadModel);

        var segmentWriter = new FileSegmentWriter(downloadModel, Path.Combine(downloadModel.DirectoryPath, downloadModel.Name), _cts);
        var dataStream = new DataStream(downloadModel, downloadStatus, config, segmentWriter, _cts);

        CheckIfDownloadFailed(downloadModel, downloadStatus);
        await dataStream.Start();

        if (downloadStatus.IsDownloadFailed)
            downloadModel.Info = null;
        else
            downloadModel.Info = ByteSizeFormatter.GetReadableByteSize(downloadModel.DownloadSize);
    }

    private async Task CheckIfDownloadFailed(DownloadModel downloadModel, DownloadStatus downloadStatus)
    {
        long tempSize;
        do
        {
            tempSize = downloadModel.StreamedSize;
            await Task.Delay(5000);
            // Successful
            if (downloadModel.StreamedSize == downloadModel.DownloadSize)
                return;

        } while (tempSize < downloadModel.StreamedSize || downloadModel.StreamedSize == 0);
        // Failed 
        downloadStatus.IsDownloadFailed = true;
        _cts.Cancel();
    }

    private static string GenerateFileName(DownloadModel downloadModel)
        => Path.GetFileNameWithoutExtension(downloadModel.URI.AbsolutePath) + MimeTypeMap.GetExtension(downloadModel.MediaType);
}
