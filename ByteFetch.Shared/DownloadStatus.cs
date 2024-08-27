using CommunityToolkit.Mvvm.ComponentModel;

namespace ByteFetch.Shared;

public partial class DownloadStatus : ObservableObject
{
    [ObservableProperty]
    private bool _isRequestHeadersFailed = false;
    [ObservableProperty]
    private bool _isFileCreationFailed = false;
    [ObservableProperty]
    private bool _isDownloadFailed = false;
    [ObservableProperty]
    private bool _isFinished = false;
}
