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
    private int _numberOfThreads = 4;
    [ObservableProperty]
    private string _saveLocation = ConfigurationServices.Get("SaveLocation");
    
}
