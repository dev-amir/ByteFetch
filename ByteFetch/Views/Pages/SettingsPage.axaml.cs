using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Diagnostics;
using System;
using ByteFetch.Config;
using Avalonia.Platform.Storage;
using ByteFetch.ViewModels;

namespace ByteFetch;

public partial class SettingsPage : UserControl
{
    private readonly SettingsPageViewModel _viewModel;
    public SettingsPage()
    {
        InitializeComponent();
        _viewModel = DataContext as SettingsPageViewModel;
    }

    private void OnResetWindowSizeClick(object sender, RoutedEventArgs e)
    {
        ConfigurationServices.Update("Height", "600");
        ConfigurationServices.Update("Width", "600");
        var exePath = Environment.ProcessPath;
        Process.Start(new ProcessStartInfo
        {
            FileName = exePath,
            UseShellExecute = true
        });
        Environment.Exit(0);
    }

    private async void OnChangeSaveLocationClick(object sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        var storageProvider = topLevel.StorageProvider;
        var saveLocationResult = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select Save Location",
            AllowMultiple = false,
        });
        if (saveLocationResult.Count > 0)
        {
            var path = saveLocationResult[0].Path.LocalPath;
            ConfigurationServices.Update("SaveLocation", path);
            _viewModel.SaveLocation = path;
        }
    }

    private void OnResetSaveLocationClick(object sender, RoutedEventArgs e)
    {
        var path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        if (path != ConfigurationServices.Get("SaveLocation"))
        {
            ConfigurationServices.Update("SaveLocation", path);
            _viewModel.SaveLocation = path;
        }
    }
}