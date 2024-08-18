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
            var downloadModel = await newDownloadDialog.ShowDialog<DownloadModel>(desktop.MainWindow);
            if (downloadModel == null) return;

            _viewModel.AllItems.Insert(0, downloadModel);
            var downloadStatus = new DownloadStatus();
            var connection = new UIToCoreConnection(_viewModel, downloadModel, downloadStatus);
            downloadStatus.PropertyChanged += connection.IsFailed_PropertyChanged;
            await connection.Bridge.StartDownload(downloadModel, downloadStatus);
        }
    }
}