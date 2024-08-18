using ByteFetch.Shared;
using System.Collections.ObjectModel;

namespace ByteFetch.ViewModels;

internal class DownloadPageViewModel
{
    public ObservableCollection<DownloadModel> AllItems { get; } = [];
}
