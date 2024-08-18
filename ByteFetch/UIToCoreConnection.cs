using ByteFetch.Core;
using ByteFetch.Shared;
using ByteFetch.ViewModels;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ByteFetch;

internal class UIToCoreConnection(DownloadPageViewModel viewModel, DownloadModel downloadModel, DownloadStatus downloadStatus)
{
    public readonly CoreServices Bridge = new CoreServices();
    private readonly DownloadPageViewModel _viewModel = viewModel;
    private readonly DownloadStatus _downloadStatus = downloadStatus;
    private readonly DownloadModel _downloadModel = downloadModel;
    private readonly int _timerSeconds = 5;
    public void IsFailed_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_downloadStatus.IsRequestHeadersFailed))
            FailedMessage("Failed to Request Headers");
        else if (e.PropertyName == nameof(_downloadStatus.IsDownloadFailed))
            FailedMessage("Failed to Download");

    }

    private void FailedMessage(string baseMessage)
    {
        int currentSecond = 0;
        Task.Run(async () =>
        {
            while (_timerSeconds > currentSecond)
            {
                _downloadModel.Name = $"{baseMessage}, Removing in {_timerSeconds - currentSecond++} second(s)";
                await Task.Delay(1000);
            }
            _viewModel.AllItems.Remove(_downloadModel);
        });
    }
}
