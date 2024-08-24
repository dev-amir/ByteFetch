using ByteFetch.Shared;

namespace ByteFetch.Core;

internal class DownloadConfig(InProgressDownloadModel inProgressDownloadModel)
{
    public readonly int NumberOfThreads = inProgressDownloadModel.NumberOfThreads;
    public readonly int BufferSize = 1024;

    public long SegmentSize
    {
        get;
        private set;
    }
    public long LastSegmentSize
    {
        get;
        private set;
    }

    public void CalculateSegmentsSizes(InProgressDownloadModel inProgressDownloadModel)
    {
        SegmentSize = inProgressDownloadModel.DownloadSize / NumberOfThreads;
        LastSegmentSize = SegmentSize + inProgressDownloadModel.DownloadSize % NumberOfThreads;
    }
}