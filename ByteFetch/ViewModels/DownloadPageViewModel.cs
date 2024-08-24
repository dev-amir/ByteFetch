using ByteFetch.Shared;
using System.Collections.ObjectModel;

namespace ByteFetch.ViewModels;

internal class DownloadPageViewModel
{
    public ObservableCollection<InProgressDownloadModel> InProgressDownloads { get; } = [];
    public ObservableCollection<FinishedDownloadModel> FinishedDownloads { get; } = [];
}
