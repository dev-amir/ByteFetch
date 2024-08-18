using ByteFetch.Shared;

namespace ByteFetch.Core;

internal class DownloadConfig(DownloadModel downloadModel)
{
    public readonly int NumberOfThreads = downloadModel.NumberOfThreads;
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

    public void CalculateSegmentsSizes(DownloadModel downloadModel)
    {
        SegmentSize = downloadModel.DownloadSize / NumberOfThreads;
        LastSegmentSize = SegmentSize + downloadModel.DownloadSize % NumberOfThreads;
    }
}