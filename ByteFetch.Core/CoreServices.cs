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
        downloadStatus.IsFileCreationFailed = !FileName.TryGenerate(inProgressDownloadModel, out string filename);
        if (downloadStatus.IsFileCreationFailed)
            return;

        inProgressDownloadModel.Name = filename;
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
}

internal static class FileName
{
    public static bool TryGenerate(InProgressDownloadModel inProgressDownloadModel, out string filename)
    {
        string fileNameWithoutExtension = GetFileNameWithoutExtension(inProgressDownloadModel, inProgressDownloadModel.Rename);
        string fileNameExtension = GetFileNameExtension(inProgressDownloadModel);
        if (File.Exists(Path.Combine(inProgressDownloadModel.DirectoryPath, fileNameWithoutExtension + fileNameExtension)))
        {
            int count = 1;
            while (File.Exists(Path.Combine(inProgressDownloadModel.DirectoryPath, $"{fileNameWithoutExtension}({count}){fileNameExtension}")))
                count++;
            fileNameWithoutExtension = $"{fileNameWithoutExtension}({count})";
        }
        filename = fileNameWithoutExtension + fileNameExtension;
        return IsValid(Path.Combine(inProgressDownloadModel.DirectoryPath, filename));
    }

    private static string GetFileNameWithoutExtension(InProgressDownloadModel inProgressDownloadModel, string rename)
    {
        string fileNameWithoutExtension = rename.Length > 0 ? rename : Path.GetFileNameWithoutExtension(inProgressDownloadModel.URI.AbsolutePath);
        if (fileNameWithoutExtension.Length > 0)
            return fileNameWithoutExtension;
        fileNameWithoutExtension = Path.GetFileNameWithoutExtension(inProgressDownloadModel.URI.AbsolutePath.TrimEnd('/'));
        if (fileNameWithoutExtension.Length > 0)
            return fileNameWithoutExtension;
        return inProgressDownloadModel.URI.Host;
    }

    private static string GetFileNameExtension(InProgressDownloadModel inProgressDownloadModel)
    {
        string fileNameExtension = Path.GetExtension(inProgressDownloadModel.URI.AbsolutePath);
        if (fileNameExtension.Length > 0)
            return fileNameExtension;
        try
        {
            return MimeTypeMap.GetExtension(inProgressDownloadModel.MediaType);
        }
        catch
        {
            return "";
        }
    }

    private static bool IsValid(string pathAndFilename)
    {
        try
        {
            using var fileStream = new FileStream(pathAndFilename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            return true;
        }
        catch
        {
            return false;
        }
    }
}