using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using ByteFetch.Dialogs;
using ByteFetch.Shared;
using ByteFetch.ViewModels;

namespace ByteFetch;

public partial class DownloadPage : UserControl
{
    private readonly DownloadPageViewModel _viewModel;
    public DownloadPage()
    {
        InitializeComponent();
        _viewModel = DataContext as DownloadPageViewModel;
    }

    private async void OnNewDownloadClick(object sender, RoutedEventArgs e)
    {
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var newDownloadDialog = new NewDownloadDialog();
            var inProgressDownloadModel = await newDownloadDialog.ShowDialog<InProgressDownloadModel>(desktop.MainWindow);
            if (inProgressDownloadModel == null) return;

            _viewModel.InProgressDownloads.Insert(0, inProgressDownloadModel);
            var downloadStatus = new DownloadStatus();
            var connection = new UIToCoreConnection(_viewModel, inProgressDownloadModel, downloadStatus);
            downloadStatus.PropertyChanged += connection.IsFailed_PropertyChanged;
            downloadStatus.PropertyChanged += connection.IsFinished_PropertyChanged;
            await connection.Bridge.StartDownload(inProgressDownloadModel, downloadStatus);
        }
    }
}