using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace ByteFetch.ViewModels;

public partial class NewDownloadDialogViewModel : ViewModelBase
{

    [ObservableProperty]
    private string _downloadURL = "";
    [ObservableProperty]
    private int _numberOfThreads = 4;
    [ObservableProperty]
    private string _targetDirectoryPath = GetDownloadFolderPath();
    static string GetDownloadFolderPath()
        => System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
}
