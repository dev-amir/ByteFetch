using ByteFetch.Shared;
using MimeTypes;

namespace ByteFetch.Core;

public class CoreServices
{
    private readonly CancellationTokenSource _cts = new();
    public async Task StartDownload(InProgressDownloadModel inProgressDownloadModel, DownloadStatus downloadStatus)
    {
        var info = new DownloadInfo(downloadStatus, inProgressDownloadModel.URI.AbsoluteUri);
        var config = new DownloadConfig(inProgressDownloadModel);

        await info.GetHeaders();
        if (downloadStatus.IsRequestHeadersFailed)
            return;

        info.ProcessHeaders(inProgressDownloadModel);
        config.CalculateSegmentsSizes(inProgressDownloadModel);
        inProgressDownloadModel.Name = GenerateFileName(inProgressDownloadModel, inProgressDownloadModel.Rename);

        var segmentWriter = new FileSegmentWriter(inProgressDownloadModel, Path.Combine(inProgressDownloadModel.DirectoryPath, inProgressDownloadModel.Name), _cts);
        var dataStream = new DataStream(inProgressDownloadModel, downloadStatus, config, segmentWriter, _cts);

        CheckIfDownloadFailed(inProgressDownloadModel, downloadStatus);
        await dataStream.Start();

        if (downloadStatus.IsDownloadFailed)
            inProgressDownloadModel.Info = null;
        else
            downloadStatus.IsFinished = true;
    }

    private async Task CheckIfDownloadFailed(InProgressDownloadModel inProgressDownloadModel, DownloadStatus downloadStatus)
    {
        long tempSize;
        do
        {
            tempSize = inProgressDownloadModel.StreamedSize;
            await Task.Delay(5000);
            // Successful
            if (inProgressDownloadModel.StreamedSize == inProgressDownloadModel.DownloadSize)
                return;

        } while (tempSize < inProgressDownloadModel.StreamedSize || inProgressDownloadModel.StreamedSize == 0);
        // Failed 
        downloadStatus.IsDownloadFailed = true;
        _cts.Cancel();
    }

    private static string GenerateFileName(InProgressDownloadModel inProgressDownloadModel, string rename)
    {
        string fileNameExtension = Path.GetExtension(inProgressDownloadModel.URI.AbsolutePath);
        string fileNameWithoutExtension = rename.Length > 0 ? rename : Path.GetFileNameWithoutExtension(inProgressDownloadModel.URI.AbsolutePath);
        if (fileNameExtension.Length > 0)
            return fileNameWithoutExtension + fileNameExtension;
        fileNameExtension = MimeTypeMap.GetExtension(inProgressDownloadModel.MediaType);
        return fileNameWithoutExtension + fileNameExtension;

    }
}
