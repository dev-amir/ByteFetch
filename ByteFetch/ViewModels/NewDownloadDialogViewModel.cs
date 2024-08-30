using ByteFetch.Config;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ByteFetch.ViewModels;

public partial class NewDownloadDialogViewModel : ViewModelBase
{

    [ObservableProperty]
    private string _downloadURL = "";
    [ObservableProperty]
    private string _rename = "";
    [ObservableProperty]
    private int _numberOfThreads = 16;
    [ObservableProperty]
    private int _minWriteSize = 1024;
    [ObservableProperty]
    private string _saveLocation = ConfigurationServices.Get("SaveLocation");
    
}
