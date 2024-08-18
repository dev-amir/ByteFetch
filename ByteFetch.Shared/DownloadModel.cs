using CommunityToolkit.Mvvm.ComponentModel;

namespace ByteFetch.Shared;

public partial class DownloadModel : ObservableObject
{
    public required string URL;
    public required string DirectoryPath;
    private string? _readableDownloadSize;
    private double _sizeQuotient;
    public required int NumberOfThreads;
    [ObservableProperty]
    private string? _name;
    [ObservableProperty]
    private string? _info;
    [ObservableProperty]
    private long _downloadSize;
    [ObservableProperty]
    private long _streamedSize;
    [ObservableProperty]
    private int _streamedPercent;
    [ObservableProperty]
    private long _writedSize;
    [ObservableProperty]
    private int _writedPercent;
    partial void OnDownloadSizeChanged(long value)
    {
        _sizeQuotient = 100 / (double)value;
        _readableDownloadSize = ByteSizeFormatter.GetReadableByteSize(value);
    }
    partial void OnStreamedSizeChanged(long value)
    {
        StreamedPercent = (int)(_sizeQuotient * value);
        Info = $"{_readableDownloadSize} / {ByteSizeFormatter.GetReadableByteSize(value)}";
    }

    partial void OnWritedSizeChanged(long value)
        => WritedPercent = (int)(_sizeQuotient * value);
}