﻿using ByteFetch.Core;
using ByteFetch.Shared;
using ByteFetch.ViewModels;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ByteFetch;

internal class UIToCoreConnection(DownloadPageViewModel viewModel, InProgressDownloadModel inProgressDownloadModel, DownloadStatus downloadStatus)
{
    public readonly CoreServices Bridge = new CoreServices();
    private readonly DownloadPageViewModel _viewModel = viewModel;
    private readonly DownloadStatus _downloadStatus = downloadStatus;
    private readonly InProgressDownloadModel _inProgressDownloadModel = inProgressDownloadModel;
    private readonly int _timerSeconds = 5;
    public void IsFailed_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_downloadStatus.IsRequestHeadersFailed))
            FailedMessage("Failed to Request Headers");
        else if (e.PropertyName == nameof(_downloadStatus.IsDownloadFailed))
            FailedMessage("Failed to Download");
    }

    public void IsFinished_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_downloadStatus.IsFinished))
        {
            var finishedDownloadModel = new FinishedDownloadModel {
                Name = _inProgressDownloadModel.Name,
                Info = ByteSizeFormatter.GetReadableByteSize(_inProgressDownloadModel.DownloadSize)
            };
            _viewModel.InProgressDownloads.Remove(_inProgressDownloadModel);
            _viewModel.FinishedDownloads.Add(finishedDownloadModel);
        }
    }

    private void FailedMessage(string baseMessage)
    {
        int currentSecond = 0;
        Task.Run(async () =>
        {
            while (_timerSeconds > currentSecond)
            {
                _inProgressDownloadModel.Name = $"{baseMessage}, Removing in {_timerSeconds - currentSecond++} second(s)";
                await Task.Delay(1000);
            }
            _viewModel.InProgressDownloads.Remove(_inProgressDownloadModel);
        });
    }
}
