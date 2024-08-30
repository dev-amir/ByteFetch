using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using ByteFetch.Shared;
using ByteFetch.ViewModels;
using System;
using System.Threading.Tasks;

namespace ByteFetch.Dialogs;

public partial class NewDownloadDialog : Window
{
    private readonly NewDownloadDialogViewModel _viewModel;
    public NewDownloadDialog()
    {
        InitializeComponent();
        _viewModel = DataContext as NewDownloadDialogViewModel;
    }

    private async void OnPasteClick(object sender, RoutedEventArgs e)
    {
        try
        {
            string text = await Clipboard.GetTextAsync();
            if (text != null && IsHttpOrHttps(text))
                _viewModel.DownloadURL = text;
            else
                ShowNotification("Clipboard Content Is Not a URL");
        }
        catch
        {
            ShowNotification("Not Supported On This Platform!");
        }

    }

    private static bool IsHttpOrHttps(string url)
        => url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

    private async void OnChangeFolderClick(object sender, RoutedEventArgs e)
    {
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            var storageProvider = topLevel.StorageProvider;
            var folderResult = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Select Save Location",
                AllowMultiple = false,
            });
            if (folderResult.Count > 0)
            {
                var selectedFolder = folderResult[0];
                _viewModel.SaveLocation = selectedFolder.Path.LocalPath;
            }
        }
    }

    private void OnStartClick(object sender, RoutedEventArgs e)
    {
        if (IsHttpOrHttps(_viewModel.DownloadURL))
            this.Close(new InProgressDownloadModel
            {
                URI = new Uri(_viewModel.DownloadURL),
                Name = "Gathering Info...",
                Rename = _viewModel.Rename,
                NumberOfThreads = _viewModel.NumberOfThreads,
                MinWriteSize = _viewModel.MinWriteSize * 1024,
                DirectoryPath = _viewModel.SaveLocation
            });
        ShowNotification("URL Must Start With HTTP/HTTPS");
    }

    private async void ShowNotification(string message)
    {
        NotificationTextBlock.Text = $"* {message}";
        await Task.Delay(3000);
        NotificationTextBlock.Text = "";
    }
}