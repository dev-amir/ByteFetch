using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByteFetch.Shared;

public partial class DownloadStatus : ObservableObject
{
    [ObservableProperty]
    private bool _isRequestHeadersFailed = false;
    [ObservableProperty]
    private bool _isDownloadFailed = false;
}
